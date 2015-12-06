using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.IpWhitelist;

namespace WebMinder.Core.RuleSets
{
    public class IpWhitelistRuleSet : CreateRule<IpWhitelistRuleSetHandler, IpAddressRequest>
    {
        public IpWhitelistRuleSet()
        {
            this.SetRule(CreateRule<IpWhitelistRuleSetHandler, IpAddressRequest>.On<IpAddressRequest>().Build().Rule);
        }
    }
}
