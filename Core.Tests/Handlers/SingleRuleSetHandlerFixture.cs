using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMinder.Core.Handlers;
using Xunit;

namespace WebMinder.Core.Tests.Handlers
{
    public class SingleRuleSetHandlerFixture : HandlerFixtureBase
    {
        static IQueryable<TestObject> _bucketOfTestObjects = new List<TestObject>().AsQueryable(); 

        [Fact]
        public void ShouldHandleCustomStorage()
        {
            const int count = 10;
            _bucketOfTestObjects = AddTestObjects(count);
            
            var ruleSetHandler = new SingleRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                Rule = testObject => testObject.IntegerProperty==444,
            };
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());

            foreach (var st in _bucketOfTestObjects)
            {
                ruleSetHandler.VerifyRule(st);
            }

            ruleSetHandler.StorageMechanism = () => _bucketOfTestObjects;
            AddTestObjects(count);
            Assert.Equal(10, _bucketOfTestObjects.Count());
            
        }

        [Fact]
        public void ShouldThrowIfWhenRuleFails()
        {
            const int count = 3;
            var ruleSetHandler = new SingleRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription
            };
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }

            const decimal bad = 2m;
            stubs.First().DecimalProperty = bad;
            ruleSetHandler.Rule = testObject => testObject.DecimalProperty == bad;

            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule());
        }

    }
}
