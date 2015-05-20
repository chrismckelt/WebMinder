using System;
using System.Linq;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.RedirectToSecureUrl;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.RuleSets;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class RuleMinderFixture
    {
        [Fact]
        public void SiteRulesBuilderWillCollectAllRulesAndRunOnVerify()
        {
            var rule1 = CreateRule<IpAddressBlockerRule, IpAddressRequest>
                .On<IpAddressRequest>()
                .With(a => a.AggregateRule = (requests => requests.Any(ip => ip.IpAddress == "111.111.111.111")))
                .Build();


            var siteRules = RuleMinder.Create()
                .AddRule<RedirectToSecureUrlRuleSet, RedirectToSecureUrl, UrlRequest>(() =>
                    new RedirectToSecureUrlRuleSet()) // predefined rule redirect all http traffic to https
                .AddRule<CreateRule<IpAddressBlockerRule, IpAddressRequest>, IpAddressBlockerRule, IpAddressRequest>(() =>
                   rule1) // custom code built rule to block a specific IP.  
                .AddRule<CreateRule<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>(() =>
                    CreateRule<UrlIsValidRule, UrlRequest>  // on the fly
                        .On<UrlRequest>()
                        .With(x => x.Rule = request => request.Url == "http://www.example.com")
                       .Build());
                
 
            Assert.Equal(3, siteRules.MindedRules.Count);
            
            siteRules.MindedRules.ToList().ForEach(ruleSet=> { if (ruleSet != null) ruleSet.VerifyRule(); });
        }
    }
}
