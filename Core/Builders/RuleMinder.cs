using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Builders
{
    public class RuleMinder 
    {
        public IList<IRuleRunner> MindedRules { get; set; }

        public static RuleMinder Create()
        {
            var minder = new RuleMinder {MindedRules = new List<IRuleRunner>()};
            return minder;
        }

        public void Add(object minded)
        {
            var casted = (IRuleRunner)minded;
            MindedRules.Add(casted);
        }

        public RuleMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<CreateRule<TRuleSetHandler2, TRuleRequest2>> setter)
            where TRuleSetHandler2 : class,IRuleSetHandler<TRuleRequest2>, new()
            where TRuleRequest2 :  RuleRequest,new()
        {
            var evaulated = setter().Rule as IRuleRunner;
            if (evaulated != null) MindedRules.Add(evaulated);
            return this;
        }
    }
}