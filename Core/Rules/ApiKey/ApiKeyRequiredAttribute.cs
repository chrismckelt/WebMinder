using System;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyRequiredAttribute : Attribute
    {
        public string HeaderApiKeyName { get; set; }

        public string HeaderApiToken { get; set; }

        public ApiKeyRequiredAttribute( )
        {
            var api = new ApiKeyRequiredRule();
            RuleSetRunner.Instance.VerifyRule(api);
        }
    }
}
