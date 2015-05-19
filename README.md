#  WebMinder

Define a custom rule set to record HTTP requests and invoke a LINQ rule to determine whether the request is valid.

Sample uses include:

- Automatically block requests from a specific IP address when a count threshold has exceeded a time duration (DDOS)
- Dynamically add IP requests to a black list & block future requests from that IP (URL vector attack)
- Ensure request is over SSL
 
## Fluent interface to build rules


            var rule = Create<IpAddressBlockerRule, IpAddressRequest>
                .On<IpAddressRequest>(request => request.IpAddress ="127.0.0.1")
                .With(y => y.RuleSetName = "NO SPAM")
                .With(x => x.ErrorDescription = "DDOS rejected")
                .Build();
                
                
	    // add the rule to your IOC container & inject & invoke on demand
	    // or use an attribute to verfiy the rule on a web api request
	    // or run it per request via global.asax - Application_BeginRequest 
	    // or add the in built module to your web.config file
	
	
## In built rules (see wiki)

    -- IpAddress blocker (sample)

		var rule = new IpAddressBlockerRule<IpAddressRequest>()
		{
			Duration = Duration = TimeSpan.FromDays(-1),
			MaxAttemptsWithinDuration = 5
		};

		Verify rule by a method attribute [IpAddressBlockerRuleVerification] or running the rule explicity

    -- Redirect to secure urls
	
	-- Url is up (200)

## 3 rule set operators

    -- AggregateRuleSetHandler - using run time arguments, filter the collection & run a predicate to find invalid items
    -- SimpleRuleSetHandler - run a predicate over the collection to determine if its valid
    -- MaximumCountRuleSetHandler - once the rule set hits this number it will trigger

## Out of the box defaults

    -- StorageMechanism is a concurrent dictionary stored in the runtime memory cache (can back onto SQL/Cache/File)
    -- HttpException 403 thrown by default with RuleSet Error Description

## Optionally add items to the collection

    RuleSetRunner.Instance.AddRule<IpAddressRequest>(new IpAddressBlockerRule(){UpdateRuleCollectionOnSuccess = false});

    UpdateRuleCollectionOnSuccess when true will add each given request to the request collection (default true)

## AggregateRuleSetHandler
    // ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
    var rule = new RuleSetHandler<IpAddressRequest>()
    {
        AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
        AggregateFilter = (data, item) => data.Where(collectionItem => collectionItem.IpAddress == item.IpAddress) // run time application for passed in IRequestRule
    };


    RuleSetRunner.Instance.AddRule<IpAddressRequest>(rule);


## SimpleRuleSetHandler

    // ip ruleset - disallow a specific IP
    var rule = new SimpleRuleSetHandler<IpAddress>()
    {
      Rule = ip => ip.IpAddress == "Some IP we dont want (or could do a range query on it)",
    };

    RuleSetRunner.Instance.AddRule(rule);


## MaximumCountRuleSetHandler (Total Count)

    // add items to this rule - once over 50 it will start rejecting requests
    var rule = new MaximumCountRuleSetHandler<IpAddress>();
    rule.MaximumResultCount = 50;

    RuleSetRunner.Instance.AddRule(rule);

## Run the rule

    // Run passing the request
    RuleSetRunner.Instance.VerifyRule(new IpAddressBlockerRule()
    {
      IpAddress = "127.0.0.1",
      CreatedUtcDateTime = DateTime.UtcNow
    });

    // or just run it - if currently is violation of rule custom invalid action will trigger
    RuleSetRunner.Instance.VerifyRule();  

## Decide what action to take when a rule is broken

	ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };

## Switch out the storage mechanism

	ruleSetHandler = new RuleSetHandler<TestObject>(ThreadData.Storage);  

## Encapsulate custom rules

  see IpAddressBlockerRule.cs

## Wire up to your own logger (log4net)
    public IpAddressBlockerRule()
    {
      WriteLog(MyCustomLogFunctionThatMapsToLog4Net);
    }

      public static void WriteLog(string category, string message)
      {
        switch (category)
        {
          case "DEBUG":
            WriteLog4NetEvent(message, Level.Debug);
            break;
          case "INFO":
            WriteLog4NetEvent(message, Level.Info);
            break;
          case "WARNING":
            WriteLog4NetEvent(message, Level.Warn);
            break;
          case "ERROR":
            WriteLog4NetEvent(message, Level.Error);
            break;
          }
    }

####  Tests in RuleSetRunnerFixture.cs have more examples

more inbuilt rules to come... (PRs welcome)
