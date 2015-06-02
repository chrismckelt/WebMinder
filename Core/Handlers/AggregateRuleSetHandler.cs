using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public class AggregateRuleSetHandler<T> : RuleSetHandlerBase<T>,
        IAggregateRuleSetHandler<T>
        where T : IRuleRequest, new()
    {
        public Expression<Func<IEnumerable<T>, T, IEnumerable<T>>> AggregateFilter { get; set; }

        public Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }

        public override void VerifyRule(IRuleRequest request = null)
        {
            base.VerifyRule(request);
            if (!StorageMechanism.Items.Any())
            {
                Logger("INFO", string.Format("Cache empty for {0}", RuleSetName));
                return;
            }
            if (AggregateRule == null)
            {
                Logger("ERROR", string.Format("Aggregate rule for {0} is empty", RuleSetName));
                return;
            }

            var filtered = AggregateFilter != null
                ? AggregateFilter.Compile().Invoke(StorageMechanism.Items, RuleRequest)
                : StorageMechanism.Items;

            var invalid = AggregateRule.Compile().Invoke(filtered);
            if (!invalid) return;
            FailRule();
        }
    }
}