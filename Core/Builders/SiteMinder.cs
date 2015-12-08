using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.ApiKey;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Runners;

namespace WebMinder.Core.Builders
{
    public sealed class SiteMinder : RuleSetRunner
    {
        public List<Exception> Failures { get; private set; }

        public static SiteMinder Create(bool addToRuleSetRunner = true)
        {
            var minder = new SiteMinder{};
            return minder;
        }

        public SiteMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<CreateRule<TRuleSetHandler2, TRuleRequest2>> setter)
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


        public SiteMinder Initialise()
        {
           Instance.Rules.AddRange(this.Rules);
            return this;
        }

        public bool AllRulesValid()
        {
           Failures = new List<Exception>();
            foreach (var rule in Rules)
            {
                try
                {
                    var runner = rule.Value as IRuleRunner;
                    runner.VerifyRule();
                }
                catch (Exception ex)
                {
                    Failures.Add(ex);
                }
            }

            return !Failures.Any();
        }

        public static void RecordBadIpRequest()
        {
            Instance.VerifyRule(IpAddressRequest.GetCurrentIpAddress(recordBadIp: true));
        }

        public static IRuleSetHandler<T> RuleFor<T>() where T : IRuleRequest,new()
        {
            return Instance.GetRules<T>().First();
        }

        public void ValidateApiKey()
        {
            Instance.VerifyRule(new ApiKeyRequiredRule());
        }

        public void ValidateWhiteList()
        {
            Instance.VerifyRule(IpAddressRequest.GetCurrentIpAddress(recordBadIp: true));
        }

        public void EnforceSsl()
        {
            Instance.VerifyRule(UrlRequest.GetCurrentUrl());
        }
    }
}