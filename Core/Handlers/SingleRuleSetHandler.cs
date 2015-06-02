using System;
using System.Linq;
using System.Linq.Expressions;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public class SingleRuleSetHandler<T> :
        RuleSetHandlerBase<T>,
        ISingleRuleSetHandler<T>
        where T : IRuleRequest, new()
    {

        public Expression<Func<T, bool>> Rule { get; set; }

        public override void VerifyRule(IRuleRequest request = null)
        {
            base.VerifyRule(request);
            if (!StorageMechanism.Items.Any()) return;
             
            if (Rule != null)
            {
                var invalid = StorageMechanism.Items
                    .AsQueryable()
                    .Cast<T>()
                    .Where(Rule);

                if (!invalid.Any()) return;
                FailRule();
            }
        }
      
    }
}