using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using FizzWare.NBuilder;
using WebMinder.Core.Handlers;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class RuleSetHandlerFixture
    {
        private const string RuleSet = "Test Rule";
        const string ErrorDescription = "Error exception for logging";
        private bool _addRequestToItemsCollection = true;

        public RuleSetHandlerFixture()
        {
            MemoryCache.Default.Remove(typeof (TestObject).Name);
        }

        [Fact]
        public void ShouldCollectDataResults()
        {
            const int count = 3;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 999,
            };
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }
            Assert.Equal(count, ruleSetHandler.Items.Count());
        }

        [Fact]
        public void ShouldNotAddToCollectionWhenUpdateRuleCollectionOnSuccessSetToFalse()
        {
            const int count = 3;
            _addRequestToItemsCollection = false;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 2,
                UpdateRuleCollectionOnSuccess = false
            };

            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs =  AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Equal(0, ruleSetHandler.Items.Count());
        }


        [Fact]
        public void ShouldThrowIfMaxCountExceeded()
        {
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 3
            };
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            const int count = 3;
            var stubs = AddTestObjects(count);
            foreach (var st in stubs.Skip(1))
            {
                ruleSetHandler.VerifyRule(st);
            }
            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule(stubs.First()));
        }


        [Fact]
        public void ShouldThrowIfWhenRuleFails()
        {
            const int count = 3;
            var ruleSetHandler = new SimpleRuleSetHandler<TestObject>
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
            stubs[0].DecimalProperty = bad;
            ruleSetHandler.Rule = testObject => testObject.DecimalProperty == bad;

            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule());
        }


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
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            int max = stubs.Max(a => a.IntegerProperty);
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
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule(TestObject.Build()));
        }

        [Fact]
        public void ShouldThrowCustomAction()
        {
            const int count = 5;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 5,
                InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); },
            };
            ruleSetHandler.UseCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs.Skip(1))
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Throws<DivideByZeroException>(() => ruleSetHandler.VerifyRule(TestObject.Build()));
        }

        static IList<TestObject> _bucketOfTestObjects = new List<TestObject>(); 

        [Fact]
        public void ShouldHandleCustomStorage()
        {
            const int count = 10;
            _bucketOfTestObjects = AddTestObjects(count);
            
            var ruleSetHandler = new SimpleRuleSetHandler<TestObject>
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
            Assert.Equal(10, _bucketOfTestObjects.Count);
            
        }

        private IList<TestObject> AddTestObjects(int count)
        {
            return Builder<TestObject>
                .CreateListOfSize(count)
                .Build();
        }

    }
}
