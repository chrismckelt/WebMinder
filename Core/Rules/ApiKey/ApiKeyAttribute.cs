using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyAttribute : Attribute
    {
        public string HeaderApiKeyName { get; set; }

        public string HeaderApiToken { get; set; }

        public ApiKeyAttribute( )
        {
            var api = new ApiKeyRuleRequest()
            {
                HeaderApiKeyName = HeaderApiKeyName,
                HeaderToken = HeaderApiToken
            };
            RuleSetRunner.Instance.VerifyRule(api);
        }
    }
}
