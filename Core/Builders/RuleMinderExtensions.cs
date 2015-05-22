using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.RedirectToSecureUrl;
using WebMinder.Core.RuleSets;

namespace WebMinder.Core.Builders
{
    public static class RuleMinderExtensions
    {
        public static RuleMinder AddRule<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder ruleMinder,
            Func<CreateRule<TRuleSetHandler, TRuleRequest>, CreateRule<TRuleSetHandler, TRuleRequest>> setter)
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : class, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : class, IRuleRequest, new()
        {
            var act = Activator.CreateInstance<TCreate>();
            var createdRule = setter.Invoke(act);
           
            if (createdRule == null) throw new EvaluateException("function to create a ruleset evaluated to null");
            Trace.WriteLine(createdRule.GetType().Name);
            ruleMinder.AddRule(typeof(TRuleRequest), createdRule.Rule);
            return ruleMinder;
        }

        public static RuleMinder WithRules(this RuleMinder ruleMinder, RuleMinder existingRuleMinder)
        {
            ruleMinder.Rules.ToList().AddRange(existingRuleMinder.Rules);
            return ruleMinder;
        }

        public static RuleMinder WithSslEnabled(this RuleMinder ruleMinder)
        {
            var ruleSet = CreateRule<RedirectToSecureUrl, UrlRequest>.On<UrlRequest>()
                .Build();

            return ruleMinder.AddRule<RedirectToSecureUrlRuleSet, RedirectToSecureUrl, UrlRequest>(x => ruleSet); // predefined rule redirect all http traffic to https
        }

        public static RuleMinder WithNoSpam(this RuleMinder ruleMinder, int? maxAttemptsWithinDuration = null,
            TimeSpan? withinDuration = null)
        {
            var ruleSet = CreateRule<IpAddressBlockerRule, IpAddressRequest>.On<IpAddressRequest>()
                .With(a => a.MaxAttemptsWithinDuration = maxAttemptsWithinDuration.GetValueOrDefault(100))
                .With(a => a.Duration = withinDuration.GetValueOrDefault(TimeSpan.FromDays(-1)))
                .Build();

            return ruleMinder.AddRule<IpBlockerRuleSet, IpAddressBlockerRule, IpAddressRequest>(x => ruleSet);
        }
    }
}