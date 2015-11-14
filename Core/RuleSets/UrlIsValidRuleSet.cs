using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.UrlIsValid;

namespace WebMinder.Core.RuleSets
{
    public class UrlIsValidRuleSet : CreateRule<UrlIsValidRuleHandler, UrlRequest>
    {
        public UrlIsValidRuleSet()
        {
            this.SetRule(CreateRule<UrlIsValidRuleHandler, UrlRequest>.On<UrlRequest>().Build().Rule);
        }
    }
}
