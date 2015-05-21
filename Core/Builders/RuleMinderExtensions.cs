using System;
using System.Diagnostics;
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
            Func<CreateRule<TRuleSetHandler, TRuleRequest>> setter)
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : class, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : class, IRuleRequest, new()
        {
            var createdRule = setter();
            var rule = createdRule;
            if (rule == null) return ruleMinder;
            Trace.WriteLine(createdRule.GetType().Name);
            ruleMinder.Add(rule.Rule);
            return ruleMinder;
        }

        public static RuleMinder WithSslEnabled(this RuleMinder ruleMinder)
        {
            var ruleSet = CreateRule<RedirectToSecureUrl, UrlRequest>
                .On<UrlRequest>()
                .Build();

            return ruleMinder.AddRule<RedirectToSecureUrlRuleSet, RedirectToSecureUrl, UrlRequest>(() => ruleSet); // predefined rule redirect all http traffic to https
        }

        public static RuleMinder WithNoSpam(this RuleMinder ruleMinder, int? maxAttemptsWithinDuration = null,
            TimeSpan? withinDuration = null)
        {
            var ruleSet = CreateRule<IpAddressBlockerRule, IpAddressRequest>
                .On<IpAddressRequest>()
                .With(a => a.MaxAttemptsWithinDuration = maxAttemptsWithinDuration.GetValueOrDefault(100))
                .With(a => a.Duration = withinDuration.GetValueOrDefault(TimeSpan.FromHours(-1)))
                .Build();

            return ruleMinder.AddRule<IpBlockerRuleSet, IpAddressBlockerRule, IpAddressRequest>(() => ruleSet);
        }
    }
}