using System;
using System.Diagnostics;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public static class RuleMinderExtensions
    {
        public static RuleMinder AddRule<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder ruleMinder, Func<CreateRule<TRuleSetHandler, TRuleRequest>> setter)
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : class, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : class,IRuleRequest, new()
        {

            var createdRule = setter();
            var rule = createdRule.Build();
            if (rule == null) return ruleMinder;
            Trace.WriteLine(createdRule.GetType().Name);
            ruleMinder.Add(rule.Rule);
            return ruleMinder;
        }

    }
}