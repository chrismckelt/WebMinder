using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyRequiredAttribute : Attribute
    {
        public string HeaderApiKeyName { get; set; }

        public string HeaderApiToken { get; set; }

        public ApiKeyRequiredAttribute( )
        {
            Debugger.Launch();
            var api = new ApiKeyRequiredRule()
            {
                HeaderApiKeyName = HeaderApiKeyName,
                HeaderToken = HeaderApiToken
            };
            RuleSetRunner.Instance.VerifyRule(api);
        }
    }
}
