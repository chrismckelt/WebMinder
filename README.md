#  Charon

### The HTTP request gatekeeper

Manage HTTP Requests based on validation rules written with LINQ

## Aggregate rules
	// ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
	var ipAnalyserRule = new RequestAnalyserRuleSet<IRuleRequest>(HttpApplicationStorage)
	{
		AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
	};


	RuleSetRunner.Instance.AddRule(ipAnalyserRule);

	
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