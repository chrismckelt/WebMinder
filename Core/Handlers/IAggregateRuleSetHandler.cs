using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public interface IAggregateRuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        Expression<Func<IEnumerable<T>, T, IEnumerable<T>>> AggregateFilter { get; set; }

        Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }
    }
}