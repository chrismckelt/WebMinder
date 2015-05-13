using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Runners;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class CreateFixture
    {
        [Fact]
        public void CreatesRuleSaysWhatItDoes()
        {
            const string rulesetname = "abcd";
            const string rulesetdesc = "xyz";

            var rule = Create<IpAddressBlockerRule, IpAddressRequest>
                .RuleSet()
                .With(y => y.RuleSetName = rulesetname)
                .With(x => x.ErrorDescription = rulesetdesc)
                .Build();

            Assert.Equal(rulesetname, rule.Rule.RuleSetName);

            Assert.Equal(rulesetdesc, rule.Rule.ErrorDescription);
        }
    }
}