#  WebMinder

### A HTTP request gatekeeper based on LINQ validation rules

## Aggregate rules
    // ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
    RuleSetHandler<IpAddressAnalyser> ipAnalyserRule = new RuleSetHandler<IpAddressAnalyser>(HttpApplicationStorage)
    {
        AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
        AggregateFilter = (data, item) => data.Where(collectionItem => collectionItem.IpAddress == item.IpAddress) // run time application for passed in IRequestRule
    };


    RuleSetRunner.Instance.AddRule<IpAddressAnalyser>(ipAnalyserRule);



## Simple rule

    // ip ruleset - disallow a specific IP
    var ipAnalyserRule = new RequestAnalyserRuleSet<IpAddressAnalyser>(HttpApplicationStorage)
    {
      Rule = ip => ip.IpAddress == "Some IP we dont want (or could do a range query on it)",
    };

    RuleSetRunner.Instance.AddRule(ipAnalyserRule);


## Total Count

    // add items to this rule - once over 50 it will start rejecting requests
    var rule = new RequestAnalyserRuleSet<IpAddressAnalyser>(HttpApplicationStorage);
    rule.MaximumResultCount = 50;

    RuleSetRunner.Instance.AddRule(ipAnalyserRule);

## Decide what action to take when a rule is broken

	ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };

## Switch out the storage mechanism

	ruleSetHandler = new RuleSetHandler<TestObject>(ThreadData.Storage);  


## Encapsulate custom rules

    public class BlockRepeatedAttemptsFromUnsuccesfulIpAddressesRule : RuleSetHandler<IpAddressRequest>
    {
	    public static TimeSpan Duration = TimeSpan.FromDays(-1);
    	public const int MaxAttemptsWithinDuration = 5;
    
	    public BlockRepeatedAttemptsFromUnsuccesfulIpAddressesRule(Func<IList<IpAddressRequest>> storageMechanism = null)
		    : base(storageMechanism)
	    {
		    RuleSetName = "Block IP Addresses with 5 or more unsuccessful tries over a 24 hour period";
    
		    ErrorDescription =
			    "If an IP Address has been used in an unsuccessful search more than 5 times in a 24 hour period, then return an unsuccessful search result (even if the search result should be a success).";
    
		    AggregateFilter = (data, item) => data.Where(x => x.IpAddress == item.IpAddress);
    
		    AggregateRule =
			    data => (from x in data.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(Duration.Ticks))
				    let observableSet = data.GroupBy(b => b.IpAddress, b => b.IpAddress, (key, c) => new
    				{
					    IpAddress = key,
    					Total = c.ToList()
				    })
    				from y in observableSet
				    where y.Total.Count > MaxAttemptsWithinDuration
    				select new IpAddressRequest
				    {
					    IpAddress = x.IpAddress
				    }).Any();
    
    
		    InvalidAction = () =>
    		{
			    var ex = new HttpException(403, ErrorDescription);
    			NewRelic.Api.Agent.NewRelic.NoticeError(ex);
			    throw ex;
		    };
	    }
    }
