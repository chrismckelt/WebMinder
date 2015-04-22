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
