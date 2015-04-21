#  Charon

### The HTTP request gatekeeper

Manage HTTP Requests based on validation rules

## Aggregate rules
	// ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
	var ipAnalyserRule = new RequestAnalyserRuleSet<IRuleRequest>(HttpApplicationStorage)
		{
			AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
		};


	RuleSetRunner.Instance.AddRule(ipAnalyserRule);

	
## Simple rule



## Total Count