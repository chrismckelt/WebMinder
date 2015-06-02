using System.Linq;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public class MaximumCountRuleSetHandler<T> :
        RuleSetHandlerBase<T>,
        IMaximumCountRuleSetHandler<T>
        where T : IRuleRequest, new()
    {
        public int? MaximumResultCount { get; set; }

        public override void VerifyRule(IRuleRequest request = null)
        {
            base.VerifyRule(request);
            if (StorageMechanism==null || !StorageMechanism.Items.Any()) return;
            long count = StorageMechanism.Items.LongCount();
            bool invalid = MaximumResultCount.HasValue && count >= MaximumResultCount;
            if (!invalid) return;
            string log = string.Format("Maximum Count excedeed.. Maximum items allowed count : {0}  total found: {1}",
                MaximumResultCount, count);
            FailRule(log);
        }
    }
}