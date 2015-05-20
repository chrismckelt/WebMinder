using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public class RuleMinder 
    {
        public IList<IRuleSetHandler<RuleRequest>> MindedRules { get; set; }

        public static RuleMinder Create()
        {
            var minder = new RuleMinder {MindedRules = new List<IRuleSetHandler<RuleRequest>>()};
            return minder;
        }

        public void Add(IRuleSetHandler<RuleRequest> minded) 
        {
           MindedRules.Add(minded);
        }

        public RuleMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<CreateRule<TRuleSetHandler2, TRuleRequest2>> setter)
            where TRuleSetHandler2 : class,IRuleSetHandler<TRuleRequest2>, new()
            where TRuleRequest2 :  RuleRequest,new()
        {
            var evaulated = setter().Rule as IRuleSetHandler<RuleRequest>;
            if (evaulated != null) MindedRules.Add(evaulated);
            return this;
        }
    }
}