using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Charon.Core;
using Charon.WebSiteTest.Rules;

namespace Charon.WebSiteTest
{

    public class MvcApplication : HttpApplication
    {

        public static RuleSetRunner RuleSetRunner { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ip ruleset - disallow more than 20 requests per day from a logged 'failed'  request
            var ipAnalyserRule = new RequestAnalyserRuleSet<IRuleRequest>(HttpApplicationStorage)
            {
                AggregateRule = ip => ip.Count(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1)) > 20,
            };


            RuleSetRunner.Instance.AddRule(ipAnalyserRule);
        }

        private IList<IRuleRequest> HttpApplicationStorage()
        {
            var results = Application["IpAnalyserRuleSet"] as IList<IRuleRequest>;
            return results ?? new List<IRuleRequest>();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            RuleSetRunner.Instance.Run(new IpAddressAnalyser()
            {
                IpAddress = "TODO - put ip address here",
                CreatedUtcDateTime = DateTime.UtcNow
            });
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
