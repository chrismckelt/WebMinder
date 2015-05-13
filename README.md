#  WebMinder

### A HTTP request gatekeeper based on LINQ validation rules

## Fluent interface to build rules


        var rule = Create<IpAddressBlockerRule, IpAddressRequest>
            .RuleSet()
            .With(y => y.RuleSetName = "Spambots")
            .With(x => x.ErrorDescription = "Deny IP addresses that are spamming us")
            .Build();

## 2 In built rules (see wiki)

    -- IpAddress blocker (sample)

    var rule = new IpAddressBlockerRule<IpAddressRequest>()
    {
        Duration = Duration = TimeSpan.FromDays(-1),
        MaxAttemptsWithinDuration = 5
    };

    Verifiy rule by a method attribute [IpAddressBlockerRuleVerification] or running the rule explicity

    -- URL Gateway circuit breaker

## 3 rule set operators

    -- AggregateRuleSetHandler
    -- SimpleRuleSetHandler
    -- MaximumCountRuleSetHandler

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
