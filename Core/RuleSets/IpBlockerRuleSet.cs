using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core.RuleSets
{
    public class IpBlockerRuleSet : CreateRule<IpAddressBlockerRule, IpAddressRequest>
    {
    }
}
