using System;
using System.Collections.Generic;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Builders
{
    public sealed class RuleMinder : RuleSetRunner
    {
        public static RuleMinder Create(bool addToRuleSetRunner = true)
        {
            var minder = new RuleMinder{};
            return minder;
        }

        public RuleMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<CreateRule<TRuleSetHandler2, TRuleRequest2>> setter)
            where TRuleSetHandler2 : class,IRuleSetHandler<TRuleRequest2>, new()
            where TRuleRequest2 :  IRuleRequest,new()
        {
            var evaulated = setter().Rule as IRuleRunner;
            if (evaulated != null)
            {
                Rules.Add(new KeyValuePair<Type, object>(typeof(TRuleRequest2), evaulated));
            }
            return this;
        }
    }
}