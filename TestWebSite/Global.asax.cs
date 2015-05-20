using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMinder.Core;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.Runners;

namespace TestWebSite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var ruleBuilder = Create<IpAddressBlockerRule, IpAddressRequest>.On<IpAddressRequest>().Build();

            var urlValid = Create<UrlIsValidRule, UrlRequest>
                .On<UrlRequest>(url=>url.Url = "http://www.google.com")
                .Build();

            RuleSetRunner.Instance.AddRule<IpAddressRequest>(ruleBuilder.Rule);
            RuleSetRunner.Instance.AddRule<UrlIsValidRule>(urlValid.Rule);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var ipaddress = GetIpAddress();

            var spamIpAddressCheck = new IpAddressRequest()
            {
                IpAddress = ipaddress,
                CreatedUtcDateTime = DateTime.UtcNow
            };

            int total =
             RuleSetRunner.Instance.GetRules<IpAddressRequest>()
                 .Sum(a => a.Items.Count(b => b.IpAddress == ipaddress));

            if (total < 2)
            {
                RuleSetRunner.Instance.VerifyRule(spamIpAddressCheck);
            }
            else
            {
                spamIpAddressCheck.IsBadRequest = true;
                RuleSetRunner.Instance.VerifyRule(spamIpAddressCheck);
            }
            var msg = GetMessage(ipaddress);
            Response.Write(msg);
        }

        private static string GetIpAddress()
        {
            var wrapper = new HttpRequestWrapper(HttpContext.Current.Request);
            string ipaddress = RequestUtility.GetClientIpAddress(wrapper);
            return ipaddress;
        }

        private static string GetMessage(string ipaddress)
        {
            int total =
                RuleSetRunner.Instance.GetRules<IpAddressRequest>()
                    .Sum(a => a.Items.Count(b => b.IpAddress == ipaddress));

            string msg = string.Format("<h1>Website hit by IP {0} a total of {1} </h1>", ipaddress, total);
            return msg;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            var httpEx = exception as HttpException;
            Response.Write("<h1>");
            if (httpEx != null && httpEx.GetHttpCode() == 403)
            {
                Response.Write(httpEx.Message);
                Response.Write("<br/>");
                var ipaddress = GetIpAddress();
                var msg = GetMessage(ipaddress);
                Response.Write(msg);
            }
            else if (httpEx != null && httpEx.GetHttpCode() == 404)
            {
                Response.Write("404 not found");
            }
            else
            {
                Response.Write("WTF");
            }
            Response.Write("</h1>");
            Response.End();
        }
    }
}