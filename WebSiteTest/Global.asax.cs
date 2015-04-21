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

            // rules sets
            var ipAnalyserRule = new RequestAnalyserRuleSet<IRuleRequest>(HttpApplicationStorage)
            {
                AggregateRule = ip => ip.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddDays(-1))
                    //.Select(b => b.IpAddress)
                    .Count() > 2,
            };


            RuleSetRunner.Instance.AddRule((RequestAnalyserRuleSet<IRuleRequest>)ipAnalyserRule);
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
