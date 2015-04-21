using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Charon.Core
{

    public interface IRuleSetHandler<TRuleFor> where TRuleFor : IRuleRequest
    {
        string RuleSetName { get; set; }
        string ErrorDescription { get; set; }

        int? MaximumResultCount { get; set; }

        IEnumerable<TRuleFor> Items { get; }

        Expression<Func<TRuleFor, bool>> Rule { get; set; }

        Expression<Func<IEnumerable<TRuleFor>, bool>> AggregateRule { get; set; }

        void Run(TRuleFor request);

        Func<IList<TRuleFor>> StorageMechanism { get; set; }

    }


}
