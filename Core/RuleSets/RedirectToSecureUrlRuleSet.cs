using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.RedirectToSecureUrl;

namespace WebMinder.Core.RuleSets
{
    public class RedirectToSecureUrlRuleSet : CreateRule<RedirectToSecureUrlRuleSetHandler, UrlRequest>
    {
        public RedirectToSecureUrlRuleSet()
        {
            this.SetRule(CreateRule<RedirectToSecureUrlRuleSetHandler, UrlRequest>.On<UrlRequest>().Build().Rule);
        }
    }
}
