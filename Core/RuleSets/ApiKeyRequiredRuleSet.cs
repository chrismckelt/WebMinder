using WebMinder.Core.Builders;
using WebMinder.Core.Rules.ApiKey;

namespace WebMinder.Core.RuleSets
{
    public class ApiKeyRequiredRuleSet : CreateRule<ApiKeyRequiredRuleSetHandler, ApiKeyRequiredRule>
    {
        public ApiKeyRequiredRuleSet()
        {
            this.SetRule(On<ApiKeyRequiredRule>().Build().Rule);
        }
    }
}
