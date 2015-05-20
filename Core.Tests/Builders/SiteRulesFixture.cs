using System.Linq;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.RedirectToSecureUrl;
using WebMinder.Core.Rules.UrlIsValid;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class SiteRulesFixture
    {
        [Fact]
        public void SiteRulesBuilderWillCollectAllRulesAndRunOnVerify()
        {
            var rule1 = Create<IpAddressBlockerRule, IpAddressRequest>
                .On<IpAddressRequest>()
                .With(a => a.AggregateRule = (requests => requests.Any(ip => ip.IpAddress == "111.111.111.111")))
                .Build();

            var rum = RuleMinder.Create()
             .And<Create<IpAddressBlockerRule, IpAddressRequest>, IpAddressBlockerRule, IpAddressRequest>(() => rule1)
             .And<Create<RedirectToSecureUrl, UrlRequest>, RedirectToSecureUrl, UrlRequest>()
             .And<Create<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>() 

              //  .And<Create<RedirectToSecureUrl, UrlRequest>, RedirectToSecureUrl, UrlRequest>() // generic out of the box rule
              //  .And<Create<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>() // another generic rule
                ;

            Assert.Equal(1, rum.MindedRules.Count);
            

        }
    }
}
