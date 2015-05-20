using System;
using System.Diagnostics;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public static class RuleMinderExtensions
    {


        public static RuleMinder AddRule<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder ruleMinder, Func<TRuleSetHandler> setter)
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : class, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : class,IRuleRequest, new()
        {

            var createdRule = setter();

            if (createdRule != null)
            {
                 Debug.WriteLine(createdRule.GetType().Name);
             //   ruleMinder.Add(createdRule); 
            }
            
            return ruleMinder;
        }

    }
}