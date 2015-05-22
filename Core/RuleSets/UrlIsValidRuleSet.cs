using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.UrlIsValid;

namespace WebMinder.Core.RuleSets
{
    public class UrlIsValidRuleSet : CreateRule<UrlIsValidRule, UrlRequest>
    {
        public UrlIsValidRuleSet()
        {
            this.SetRule(CreateRule<UrlIsValidRule, UrlRequest>.On<UrlRequest>().Build().Rule);
        }
    }
}
