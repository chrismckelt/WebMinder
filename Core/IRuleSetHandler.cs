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

        IEnumerable<IRuleRequest> Items { get; }

        Expression<Func<T, bool>> Rule { get; set; }

        Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }

        Func<IList<IRuleRequest>> StorageMechanism { get; set; }

    }
}
