#  WebMinder

A simple software firewall to ensure incoming HTTP requests conform to a predefined set of rules which may be managed on the fly.

usuages:

- Block requests from a specific IP address when a threshold exceeds  (DDOS)
- Dynamically add IP requests to a black list & block future requests from that IP (URL vector attack)
- Ensure request is over SSL (redirect to SSL if otherwise)


## Out of the box HTTP Module for IP Blocking

    <modules>
       <add name="SiteMinderModule" type="WebMinder.Core.SiteMinderModule, WebMinder.Core" />
     </modules>


![image](https://cloud.githubusercontent.com/assets/662868/8300402/1a6a9e34-19b7-11e5-9a5c-b740f06e2354.png)

###    Dont like the current HTTP Request?  

SiteMinder.RecordBadIpRequest();

(default rule below is for 5 bad request per IP per hour)

## Fluent interface for individual rules

    // ensure an external service is up before continuing an action
    var urlValid = CreateRule<UrlIsValidRule, UrlRequest>
        .On<UrlRequest>(url => url.Url = "http://www.externalservice.com/monitoring/api")
        .Build();

        SiteMinder = RuleMinder.Create()
            .WithSslEnabled()
            .WithNoSpam(5, TimeSpan.FromHours(1))
            .AddRule<CreateRule<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>(() =>
                urlValid);


## In built rules (see wiki)

    - IpAddress blocker (sample with non fluent creation )
    - Redirect to secure urls (site must be SSL)
	  - Url is valid  (checks given url gives a 200 or trips action)

## Built in RuleSets

    - AggregateRuleSetHandler - using run time arguments, filter the collection & run a predicate to find invalid items
    - SingleRuleSetHandler - run a predicate over the collection to determine if its valid
    - MaximumCountRuleSetHandler - once the rule set hits this number it will trigger

## Out of the box defaults

    -- Storage uses the runtime memory cache (or optionally write to an xml file)
    -- HttpException 403 thrown by default with RuleSet Error Description


## Query over data
#### AggregateRuleSetHandler
    // ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
    var rule = new RuleSetHandler<IpAddressRequest>()
    {
        AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
        AggregateFilter = (data, item) => data.Where(collectionItem => collectionItem.IpAddress == item.IpAddress) // run time application for passed in IRequestRule
    };


    RuleSetRunner.Instance.AddRule<IpAddressRequest>(rule);

#### SingleRuleSetHandler

    // ip ruleset - disallow a specific IP
    var rule = new SingleRuleSetHandler<IpAddress>()
    {
      Rule = ip => ip.IpAddress == "Some IP we dont want (or could do a range query on it)",
    };

    RuleSetRunner.Instance.AddRule(rule);

#### MaximumCountRuleSetHandler (Total Count)

    // add items to this rule - once over 50 it will start rejecting requests
    var rule = new MaximumCountRuleSetHandler<IpAddress>();
    rule.MaximumResultCount = 50;

    RuleSetRunner.Instance.AddRule(rule);

## Verify a rule

  SiteMinder.VerifyRule(spamIpAddressCheck);

## Decide what action to take when a rule is broken

	ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };

## Choose your storage (memory cache or xml file)

IStorageProvider<T> implementations (defaults to memory)

- MemoryCacheStorageProvider
- XmlFileStorageProvider

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

####  see tests for example usage

## Structure of components

  -   SiteMinder
    -  RuleMinders
        - RuleSets (actionable LINQ queries over a custom type rule request)
          - Rules


### Demo site below sample to throw a 403 if the same IP address attempts more than 5 requests

![image](https://cloud.githubusercontent.com/assets/662868/8301367/e1b3ad98-19bf-11e5-808b-cf874c871ef3.png)

## Some sample code (not using HTTP Module) in Global.asax

![image](https://cloud.githubusercontent.com/assets/662868/7762569/6d431512-0069-11e5-9b06-3e74bcf84a6d.png)
