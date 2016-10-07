using System;
using System.Linq;
using System.Web;
using WebMinder.Core.Handlers;
using Xunit;

namespace WebMinder.Core.Tests.Handlers
{
    public class AggregateRuleSetHandlerTests : HandlerFixtureBase
    {
        [Fact]
        public void ShouldThrowIfWhenAggregateRuleFails()
        {
            const int count = 5;
            var ruleSetHandler = new AggregateRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                AggregateRule = testObject => testObject.Any()
            };
            ruleSetHandler.UseMemoryCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            var max = stubs.Max(a => a.IntegerProperty);
            ruleSetHandler.AggregateFilter = (data, item) => data.Where(a => a.IntegerProperty <= max); //runtime filter

            foreach (var st in stubs)
            {
                Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule(st));
            }
        }

        [Fact]
        public void ShouldThrowIfWhenAggregateFilterAppliedFails()
        {
            const int count = 5;
            var ruleSetHandler = new AggregateRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                AggregateRule = testObject => testObject.Count() > 5
            };
            ruleSetHandler.UseMemoryCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule(TestObject.Build()));
        }
    }
}