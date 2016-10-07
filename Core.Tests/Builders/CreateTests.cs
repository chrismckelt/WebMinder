using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class CreateTests
    {
        [Fact]
        public void CreatesRuleSaysWhatItDoes()
        {
            const string rulesetname = "abcd";
            const string rulesetdesc = "xyz";

            var rule = CreateRule<IpAddressBlockerRule, IpAddressRequest>
                .On<IpAddressRequest>(request => request.IpAddress ="127.0.0.1")
                .With(y => y.RuleSetName = rulesetname)
                .With(x => x.ErrorDescription = rulesetdesc)
                .Build();

            Assert.Equal(rulesetname, rule.Rule.RuleSetName);

            Assert.Equal(rulesetdesc, rule.Rule.ErrorDescription);
        }
    }
}