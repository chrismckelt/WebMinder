using System;
using System.Linq;
using WebMinder.Core.Rules;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class RuleSetRunnerFixture
    {
        private IRuleSetHandler<TestObject> _ruleSetHandler;
        private const string RuleSet = "RuleSetRunnerFixture Test Rule";
        const string ErrorDescription = "RuleSetRunnerFixture Error exception for logging";

        public RuleSetRunnerFixture()
        {
            _ruleSetHandler = new RuleSetHandler<TestObject>();

        }

        [Fact]
        public void ShouldGetRulesFromRuleSets()
        {
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(new IpAddressBlockerRule());

            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(1, rules.Count());
            
        }

        [Fact]
        public void ShouldGetRulesCountFromRuleSets()
        {
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(new IpAddressBlockerRule());
            RuleSetRunner.Instance.Run(new IpAddressRequest(){IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow});
            RuleSetRunner.Instance.Run(new IpAddressRequest() { IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow });
            RuleSetRunner.Instance.Run(new IpAddressRequest() { IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow });
            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(3, rules.Sum(x=>x.Items.Count()));

        }
    }
}
