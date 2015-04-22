using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using FizzWare.NBuilder;
using Xunit;

namespace WebMinder.Core.Tests
{
    public class RuleSetHandlerFixture
    {
        private IRuleSetHandler<TestObject> _ruleSetHandler;
        private IList<TestObject> _testObjects;
        private const string RuleSet = "Test Rule";
        const string ErrorDescription = "Error exception for logging";
        private bool _addRequestToItemsCollection = true;

        public RuleSetHandlerFixture()
        {

            _ruleSetHandler = new RuleSetHandler<TestObject>();
            _ruleSetHandler.RuleSetName = RuleSet;
            _ruleSetHandler.ErrorDescription = ErrorDescription;

        }

        [Fact]
        public void ShouldCollectDataResults()
        {
            const int count = 3;
            AddTestObjects(count);

            Assert.Equal(count, _ruleSetHandler.Items.Count());
        }

        [Fact]
        public void ShouldNotAddToCollectionWhenDataResults()
        {
            const int count = 3;
            _addRequestToItemsCollection = false;
            AddTestObjects(count);

            Assert.Equal(0, _ruleSetHandler.Items.Count());
        }


        [Fact]
        public void ShouldThrowIfMaxCountExceeded()
        {
            _ruleSetHandler.MaximumResultCount = 2;
            const int count = 3;
            Assert.Throws<HttpException>(() =>  AddTestObjects(count));
        }


        [Fact]
        public void ShouldThrowIfWhenRuleFails()
        {
            const int count = 3;
            AddTestObjects(count);
            const decimal bad = 2m;
            _testObjects[0].DecimalProperty = bad;
            _ruleSetHandler.Rule = testObject => testObject.DecimalProperty == bad;

            Assert.Throws<HttpException>(() => _ruleSetHandler.Run(TestObject.Build()));
        }


        [Fact]
        public void ShouldThrowIfWhenAggregateRuleFails()
        {
            const int count = 10;
            AddTestObjects(count);
            int max = _testObjects.Max(a => a.IntegerProperty);
            _ruleSetHandler.AggregateFilter = (data,item) => data.Where(a => a.IntegerProperty == item.IntegerProperty && item.IntegerProperty == max);
            _ruleSetHandler.AggregateRule = testObject => testObject.Count() > 5;

            Assert.DoesNotThrow(() => _ruleSetHandler.Run(TestObject.Build()));
        }


        [Fact]
        public void ShouldThrowIfWhenAggregateFilterAppliedFails()
        {
            const int count = 10;
            AddTestObjects(count);
            _ruleSetHandler.AggregateRule = testObject => testObject.Count() > 5;

            Assert.Throws<HttpException>(() => _ruleSetHandler.Run(TestObject.Build()));
        }


        [Fact]
        public void ShouldThrowCustomAction()
        {
            const int count = 10;
            AddTestObjects(count);
            _ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };
            _ruleSetHandler.AggregateRule = testObject => testObject.Count() > 5;

            Assert.Throws<DivideByZeroException>(() => _ruleSetHandler.Run(TestObject.Build()));
        }

        [Fact]
        public void ShouldHandleCustomStorage()
        {
            const int count = 10;
            _ruleSetHandler = new RuleSetHandler<TestObject>(ThreadData.Storage);
            AddTestObjects(count);
            _ruleSetHandler.InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); };
            _ruleSetHandler.AggregateRule = testObject => testObject.Count() > 5;

            Assert.Throws<DivideByZeroException>(() => _ruleSetHandler.Run(TestObject.Build()));
        }


        private void AddTestObjects(int count)
        {
            _testObjects = Builder<TestObject>
                .CreateListOfSize(count)
                .Build();

            foreach (var to in _testObjects)
            {
                _ruleSetHandler.Run(to, _addRequestToItemsCollection);
            }
        }

        internal class ThreadData
        {
            [ThreadStaticAttribute] public static Func<IList<TestObject>> _threadCache;

            public static Func<IList<TestObject>> Storage
            {
                get
                {
                    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    return _threadCache;
                }
                set
                {
                    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    _threadCache = value;
                }
            }
        }
    }
}
