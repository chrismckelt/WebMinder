using System;
using System.Security.Policy;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.UrlIsValid;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class RuleMinderFixture
    {
        [Fact]
        public void SiteRulesBuilderWillCollectAllRulesAndRunOnVerify()
        {

            // Fluent builder to add many custom or inbuilt rules in  global.asax application_onstart
            var siteMinder = RuleMinder.Create()
                .WithSslEnabled() // predefined rule redirect all http traffic to https
                .WithNoSpam(maxAttemptsWithinDuration: 100, withinDuration: TimeSpan.FromHours(1))
                .AddRule<CreateRule<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>(x =>
                    x.Using<UrlRequest>(request => request.Url = ("/SampleWebServiceEndPoint"))
                    .Build());

            //siteMinder.VerifyAllRules();  // global.asax  run via Application_BeginRequest 

            Assert.Equal(3, siteMinder.Rules.Count);

           
        }
    }
}