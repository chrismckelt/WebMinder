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

## Run the rule

    RuleSetRunner.Instance.Run(new IpAddressAnalyser()
    {
      IpAddress = "127.0.0.1",
      CreatedUtcDateTime = DateTime.UtcNow
    });

## Decide what action to take when a rule is broken

	ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };

## Switch out the storage mechanism

	ruleSetHandler = new RuleSetHandler<TestObject>(ThreadData.Storage);  


## Encapsulate custom rules

    public class IpAddressBlockerRule :  RuleSetHandler<IpAddressRequest>
    {
        public TimeSpan? Duration { get; set; }
        public int? MaxAttemptsWithinDuration { get; set; }

        public IpAddressBlockerRule()
        {
            RuleSetName = "Block IP Addresses with 5 or more unsuccessful tries over a 24 hour period";

            ErrorDescription =
            "If an IP Address has been used in an unsuccessful search more than 5 times in a 24 hour period, then return an unsuccessful search result (even if the search result should be a success).";

            if (!Duration.HasValue) Duration = TimeSpan.FromDays(-1);
            if (!MaxAttemptsWithinDuration.HasValue) MaxAttemptsWithinDuration = 5;

            AggregateFilter = (data, item) => data.Where(x => x.IpAddress == item.IpAddress);

            AggregateRule =
                data => (from x in data.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(Duration.Value.Ticks))
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
                var ex = new HttpException(403, string.Format("{0}  Bad IP Address: {1}", RuleSetName, RuleRequest.IpAddress));
                throw ex;
            };
            Log(WriteLog);
         }

         private void WriteLog(string catergory, string message)
         {
             switch (catergory)
             {
                 case "DEBUG":
                     Logger.Debug(message);
                     break;
                 case "INFO":
                     Logger.Info(message);
                     break;
                 case "WARNING":
                     Logger.Warn(message);
                     break;
                 case "ERROR":
                     Logger.Error(message);
                     break;
             }
         }
     }
