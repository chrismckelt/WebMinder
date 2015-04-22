using System;
using System.Collections.Generic;
using System.Linq;
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

            Assert.Equal(count, _testObjects.Count);
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
            _ruleSetHandler.AggregateRule = testObject => testObject.Count() > 5;

            Assert.Throws<HttpException>(() => _ruleSetHandler.Run(TestObject.Build()));
        }


        private void AddTestObjects(int count)
        {
            _testObjects = Builder<TestObject>.CreateListOfSize(count).Build();

            foreach (var to in _testObjects)
            {
                _ruleSetHandler.Run(to);
            }
        }
    }
}
