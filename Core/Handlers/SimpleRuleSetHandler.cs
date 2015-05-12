using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebMinder.Core.Handlers
{
    public class SimpleRuleSetHandler<T> :
        RuleSetHandlerBase<T>,
        ISimpleRuleSetHandler<T>
        where T : IRuleRequest, new()
    {

        public Expression<Func<T, bool>> Rule { get; set; }

        public override void VerifyRule(IRuleRequest request = null)
        {
            base.VerifyRule(request);
            if (!StorageMechanism().Any()) return;
             
            if (Rule != null)
            {
                var invalid = StorageMechanism()
                    .AsQueryable()
                    .Cast<T>()
                    .Where(Rule);

                if (invalid.Any())
                {
                    _logger("WARN", "Rule Failed SimpleRuleSetHandler");
                    InvalidAction();
                }
            }
        }

       
    }
}