using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.ApiKey;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.IpWhitelist;
using WebMinder.Core.Rules.RedirectToSecureUrl;
using WebMinder.Core.RuleSets;
using WebMinder.Core.StorageProviders;
using WebMinder.Core.Utilities;

namespace WebMinder.Core.Builders
{
    public static class SiteMinderExtensions
    {
        private static object _keylock = new object();
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

        public static SiteMinder WithRules(this SiteMinder siteMinder, SiteMinder existingRuleMinder)
        {
            siteMinder.Rules.ToList().AddRange(existingRuleMinder.Rules);
            return siteMinder;
        }

        public static SiteMinder WithSslEnabled(this SiteMinder siteMinder)
        {
            lock (siteMinder)
            {
                var ruleSet = CreateRule<RedirectToSecureUrlRuleSetHandler, UrlRequest>.On<UrlRequest>().Build();
                return siteMinder.AddRule<RedirectToSecureUrlRuleSet, RedirectToSecureUrlRuleSetHandler, UrlRequest>(x => ruleSet); // predefined rule redirect all http traffic to https   
            }
        }

        public static SiteMinder WithNoSpam(this SiteMinder siteMinder, int? maxAttemptsWithinDuration = null,
            TimeSpan? withinDuration = null)
        {
            lock (_keylock)
            {
                var fileStorage = new XmlFileStorageProvider<IpAddressRequest>();
                fileStorage.Initialise(new[] { GetXmlFolder() });
                var ruleSet = CreateRule<IpAddressBlockerRuleSetHandler, IpAddressRequest>.On<IpAddressRequest>()
                    .With(a => a.MaxAttemptsWithinDuration = maxAttemptsWithinDuration.GetValueOrDefault(100))
                    .With(a => a.Duration = withinDuration.GetValueOrDefault(TimeSpan.FromDays(-1)))
                    .With(a => a.StorageMechanism = fileStorage)
                    .Build();

                return siteMinder.AddRule<IpBlockerRuleSet, IpAddressBlockerRuleSetHandler, IpAddressRequest>(x => ruleSet);
            }
        }

        public static SiteMinder WithApiKeyValidation(this SiteMinder siteMinder)
        {
            lock (_keylock)
            {
                string headerApiKeyName = ConfigurationManager.AppSettings["WebMinder.ApiKeyName"];
                string headerApiKeyValue = ConfigurationManager.AppSettings["WebMinder.ApiKeyValue"];

                Guard.AgainstNull(headerApiKeyName, "WebMinder.ApiKeyName value null or empty in the configuration file");
                Guard.AgainstNull(headerApiKeyValue, "WebMinder.ApiKeyValue value null or empty in the configuration file");

                var fileStorage = new MemoryCacheStorageProvider<ApiKeyRequiredRule>();
                fileStorage.Initialise(new[] { GetXmlFolder() });
                var ruleSet = CreateRule<ApiKeyRequiredRuleSetHandler, ApiKeyRequiredRule>.On<ApiKeyRequiredRule>()
                    .With(a => a.HeaderKeyName = headerApiKeyName)
                    .With(a => a.HeaderKeyValue = headerApiKeyValue)
                    .Build();

                return siteMinder.AddRule<ApiKeyRequiredRuleSet, ApiKeyRequiredRuleSetHandler, ApiKeyRequiredRule>(x => ruleSet);
            }
        }

        public static SiteMinder WithIpWhitelist(this SiteMinder siteMinder)
        {
            lock (_keylock)
            {
                string validIpRanges = ConfigurationManager.AppSettings["WebMinder.IpWhitelist.ValidIpRanges"];

                if (string.IsNullOrEmpty(validIpRanges) || validIpRanges.Contains("*"))
                {
                    return siteMinder;
                }

                Guard.AgainstNull(validIpRanges, "WebMinder.IpWhitelist.ValidIpRanges value null or empty in the configuration file");

                var ipRanges = new Dictionary<string, string>();

                if (validIpRanges.Contains(Convert.ToChar(";")))
                {
                    var ranges = validIpRanges.Split(Convert.ToChar(";"));
                    try
                    {
                        if (ranges.Count() == 1)
                        {
                            var singleRange = validIpRanges.Split(Convert.ToChar("|"));
                            ipRanges.Add(singleRange[0], singleRange[1]);
                        }
                        else
                        {
                            foreach (var range in ranges)
                            {
                                if (!range.Contains(Convert.ToChar("|")))
                                {
                                    ipRanges.Add(range, range);
                                }
                                else
                                {
                                    var singleRange = range.Split(Convert.ToChar("|"));
                                    ipRanges.Add(singleRange[0], singleRange[1]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //<add key="WebMinder.IpWhitelist.ValidIpRanges" value="127.0.0.1;191.239.187.149|191.239.187.149"/>
                        throw new FormatException("WebMinder.IpWhitelist.ValidIpRanges config value must contain ; to split the IP range.  To add more use |  - 127.0.0.1;191.239.187.149|191.239.187.149    - or use * for all", ex);
                    }
                }
                else
                {
                    ipRanges.Add(validIpRanges, validIpRanges);
                }

                var fileStorage = new MemoryCacheStorageProvider<ApiKeyRequiredRule>();
                fileStorage.Initialise(new[] { GetXmlFolder() });
                var ruleSet = CreateRule<IpWhitelistRuleSetHandler, IpAddressRequest>.On<IpAddressRequest>()
                    .With(x => x.ValidIpRanges = ipRanges)
                    .Build();

                return siteMinder.AddRule<IpWhitelistRuleSet, IpWhitelistRuleSetHandler, IpAddressRequest>(x => ruleSet);
            }
        }

        private static string GetXmlFolder()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
            catch (Exception)
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
    }
}