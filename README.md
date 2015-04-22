#  WebMinder

### A HTTP request gatekeeper based on LINQ validation rules

## Aggregate rules
	// ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
	var ipAnalyserRule = new RequestAnalyserRuleSet<IRuleRequest>(HttpApplicationStorage)
	{
		AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
	};


	RuleSetRunner.Instance.AddRule(ipAnalyserRule);

	
## Simple rule

        // ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
            RuleSetHandler<IpAddressAnalyser> ipAnalyserRule = new RuleSetHandler<IpAddressAnalyser>(HttpApplicationStorage)
            {
                AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 2,
                AggregateFilter = (data, item) => data.Where(collectionItem => collectionItem.IpAddress == item.IpAddress)
            };


            RuleSetRunner.Instance.AddRule<IpAddressAnalyser>(ipAnalyserRule);

## Total Count

	// add items to this rule - once over 50 it will start rejecting requests
	var rule = new RequestAnalyserRuleSet<IpAddressAnalyser>(HttpApplicationStorage);
	rule.MaximumResultCount = 50;

	RuleSetRunner.Instance.AddRule(ipAnalyserRule);
