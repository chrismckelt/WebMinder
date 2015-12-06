﻿using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.ApiKey;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.RedirectToSecureUrl;
using WebMinder.Core.RuleSets;
using WebMinder.Core.StorageProviders;
using WebMinder.Core.Utilities;

namespace WebMinder.Core.Builders
{
    public static class SiteMinderExtensions
    {
        public static SiteMinder AddRule<TCreate, TRuleSetHandler, TRuleRequest>(this SiteMinder ruleMinder,
            Func<CreateRule<TRuleSetHandler, TRuleRequest>, CreateRule<TRuleSetHandler, TRuleRequest>> setter = null)
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : class, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : class, IRuleRequest, new()
        {
            var act = Activator.CreateInstance<TCreate>();
            var createdRule = setter?.Invoke(act);
           
            if (createdRule == null) throw new EvaluateException("function to create a ruleset evaluated to null");
            Trace.WriteLine(createdRule.GetType().Name);
            ruleMinder.AddRule(typeof(TRuleRequest), createdRule.Rule);
            return ruleMinder;
        }

        public static SiteMinder WithRules(this SiteMinder ruleMinder, SiteMinder existingRuleMinder)
        {
            ruleMinder.Rules.ToList().AddRange(existingRuleMinder.Rules);
            return ruleMinder;
        }

        public static SiteMinder WithSslEnabled(this SiteMinder ruleMinder)
        {
            var ruleSet = CreateRule<RedirectToSecureUrlRuleSetHandler, UrlRequest>.On<UrlRequest>()
                .Build();

            return ruleMinder.AddRule<RedirectToSecureUrlRuleSet, RedirectToSecureUrlRuleSetHandler, UrlRequest>(x => ruleSet); // predefined rule redirect all http traffic to https
        }

        public static SiteMinder WithNoSpam(this SiteMinder ruleMinder, int? maxAttemptsWithinDuration = null,
            TimeSpan? withinDuration = null)
        {
            var fileStorage = new XmlFileStorageProvider<IpAddressRequest>();
            fileStorage.Initialise(new[] { AppDomain.CurrentDomain.BaseDirectory});
            var ruleSet = CreateRule<IpAddressBlockerRuleSetHandler, IpAddressRequest>.On<IpAddressRequest>()
                .With(a => a.MaxAttemptsWithinDuration = maxAttemptsWithinDuration.GetValueOrDefault(100))
                .With(a => a.Duration = withinDuration.GetValueOrDefault(TimeSpan.FromDays(-1)))
                .With(a=>a.StorageMechanism = fileStorage)
                .Build();

            return ruleMinder.AddRule<IpBlockerRuleSet, IpAddressBlockerRuleSetHandler, IpAddressRequest>(x => ruleSet);
        }

        public static SiteMinder WithApiKeyValidation(this SiteMinder ruleMinder)
        {
            string headerApiKeyName =   ConfigurationManager.AppSettings["WebMinder.ApiKeyName"];
            string headerApiKeyValue = ConfigurationManager.AppSettings["WebMinder.ApiKeyValue"];

            Guard.AgainstNull(headerApiKeyName, "WebMinder.ApiKeyName value null or empty in the configuration file");
            Guard.AgainstNull(headerApiKeyValue, "WebMinder.ApiKeyValue value null or empty in the configuration file");

            var fileStorage = new MemoryCacheStorageProvider<ApiKeyRequiredRule>();
            fileStorage.Initialise(new[] { AppDomain.CurrentDomain.BaseDirectory });
            var ruleSet = CreateRule<ApiKeyRequiredRuleSetHandler, ApiKeyRequiredRule>.On<ApiKeyRequiredRule>()
                .With(a => a.HeaderKeyName = headerApiKeyName)
                .With(a=>a.HeaderKeyValue = headerApiKeyValue)
                .Build();

            return ruleMinder.AddRule<ApiKeyRequiredRuleSet, ApiKeyRequiredRuleSetHandler, ApiKeyRequiredRule>(x => ruleSet);
        }
    }
}