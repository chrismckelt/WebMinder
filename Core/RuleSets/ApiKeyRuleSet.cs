using WebMinder.Core.Builders;
using WebMinder.Core.Rules.ApiKey;

namespace WebMinder.Core.RuleSets
{
    public class ApiKeyRuleSet : CreateRule<ApiKeyRuleSetHandler, ApiKeyRuleRequest>
    {
        public ApiKeyRuleSet()
        {
            SetRule(On<ApiKeyRuleRequest>().Build().Rule);
        }
    }
}

