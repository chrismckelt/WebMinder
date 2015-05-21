using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core.RuleSets
{
    public class IpBlockerRuleSet : CreateRule<IpAddressBlockerRule, IpAddressRequest>
    {
        public IpBlockerRuleSet()
        {
            this.SetRule(CreateRule<IpAddressBlockerRule, IpAddressRequest>.On<IpAddressRequest>().Build().Rule);
        }
    }
}
