using System.Collections.Generic;
using System.Runtime.Caching;
using FizzWare.NBuilder;

namespace WebMinder.Core.Tests.Handlers
{
    public abstract class HandlerFixtureBase
    {
        protected const string RuleSet = "Test Rule";
        protected const string ErrorDescription = "Error exception for logging";
        protected bool AddRequestToItemsCollection = true;

        protected HandlerFixtureBase()
        {
            MemoryCache.Default.Remove(typeof(TestObject).Name);
        }

        protected IList<TestObject> AddTestObjects(int count)
        {
            return Builder<TestObject>
                .CreateListOfSize(count)
                .Build();
        }

    }
}
