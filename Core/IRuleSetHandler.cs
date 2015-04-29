using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebMinder.Core
{
    public interface IRuleSetHandler<T> : IRuleRunner where T : IRuleRequest, new()
    {
        string RuleSetName { get; set; }

        string ErrorDescription { get; set; }

        Type RuleType { get; }

        int? MaximumResultCount { get; set; }

        IEnumerable<T> Items { get; }

        Expression<Func<T, bool>> Rule { get; set; }

        T RuleRequest { get; }

        Expression<Func<IEnumerable<T>,T, IEnumerable<T>>> AggregateFilter { get; set; }
        
        Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }

        Func<IList<T>> StorageMechanism { get; set; }

        Action InvalidAction { get; set; }

        void UseCacheStorage();

        void AddExistingItems(IEnumerable<T> existingItems);

        void Log(Action<string, string> logger);
    }
}
