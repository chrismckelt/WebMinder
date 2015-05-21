using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.RedirectToSecureUrl;

namespace WebMinder.Core.RuleSets
{
    public class RedirectToSecureUrlRuleSet : CreateRule<RedirectToSecureUrl, UrlRequest>
    {
        public RedirectToSecureUrlRuleSet()
        {
            this.SetRule(CreateRule<RedirectToSecureUrl, UrlRequest>.On<UrlRequest>().Build().Rule);
        }
    }
}
