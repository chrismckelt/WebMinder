using System;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.RuleSets;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class RuleMinderFixture
    {
        [Fact]
        public void RuleMinderAddsExistingRuleMinder()
        {
            var existingRuleMinder = RuleMinder.Create().AddRule<UrlIsValidRuleSet, UrlIsValidRule, UrlRequest>(x =>
                x.Using<UrlRequest>(request => request.Url = ("/SampleWebServiceEndPoint"))
                    .Build());

            Assert.Equal(1, existingRuleMinder.Rules.Count);
        }

        [Fact]
        public void RuleMinderAddsExistingRule()
        {
            var ruleSet =
                CreateRule<UrlIsValidRule, UrlRequest>.On<UrlRequest>(request => request.Url = "/SomeWebService")
                    .Build();

            var rm =
                RuleMinder.Create()
                    .AddRule<UrlIsValidRuleSet, UrlIsValidRule, UrlRequest>(webServiceUpRuleSet => ruleSet);

            Assert.Equal(1, rm.Rules.Count);
        }


        [Fact]
        public void RuleMinderCanChainRules()
        {
            // Fluent builder to add many custom or inbuilt rules in  global.asax application_onstart
            var siteMinder = RuleMinder.Create()
                .WithSslEnabled() // predefined rule redirect all http traffic to https
                .WithNoSpam(maxAttemptsWithinDuration: 100, withinDuration: TimeSpan.FromHours(1));

            siteMinder.VerifyAllRules(); // global.asax  run via Application_BeginRequest 

            Assert.Equal(2, siteMinder.Rules.Count);
        }
    }
}