using System;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyRuleRequest : RuleRequest
    {
        public string HeaderApiKeyName { get; set; }

        public string HeaderToken { get; set; }
       
    }
}
