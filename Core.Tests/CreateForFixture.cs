using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Runners;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class CreateForFixture
    {
        [Fact]
        public void CreatesRuleSaysWhatItDoes()
        {
            const string rulesetname = "abcd";
            const string rulesetdesc = "xyz";

            var rule = CreateRule<IpAddressBlockerRule, IpAddressRequest>
                .For()
                .With(y => y.RuleSetName = rulesetname)
                .With(x => x.ErrorDescription = rulesetdesc)
                .Build();

            Assert.Equal(rulesetname, rule.Rule.RuleSetName);

            Assert.Equal(rulesetname, rule.Rule.RuleSetName);
        }
    }
}