using System;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Runners;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class RuleSetRunnerFixture
    {
        private AggregateRuleSetHandler<TestObject> _ruleSetHandler;
        private const string RuleSet = "RuleSetRunnerFixture Test Rule";
        const string ErrorDescription = "RuleSetRunnerFixture Error exception for logging";

        public RuleSetRunnerFixture()
        {
            _ruleSetHandler = new AggregateRuleSetHandler<TestObject>();

        }

        [Fact]
        public void ShouldGetRulesFromRuleSets()
        {
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(new IpAddressBlockerRule(){UpdateRuleCollectionOnSuccess = false});

            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(1, rules.Count());
            
        }

        [Fact]
        public void ShouldGetRulesCountFromRuleSets()
        {
            var rule = new IpAddressBlockerRule();
            rule.UseCacheStorage(Guid.NewGuid().ToString());
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(rule);
           
            rule.VerifyRule(new IpAddressRequest() { IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            Assert.Equal(1, rule.Items.Count());
            
            rule.VerifyRule(new IpAddressRequest() { IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            Assert.Equal(2, rule.Items.Count());
             
            rule.VerifyRule(new IpAddressRequest() { IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(3, rules.Sum(x=>x.Items.Count()));

        }
    }
}
