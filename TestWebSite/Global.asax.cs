using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebGrease.Css.Extensions;
using WebMinder.Core;
using WebMinder.Core.Builders;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.RuleSets;
using WebMinder.Core.Runners;

namespace TestWebSite
{
    public class MvcApplication : HttpApplication
    {
        public static RuleMinder SiteMinder;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var urlValid = CreateRule<UrlIsValidRule, UrlRequest>
            .On<UrlRequest>(url => url.Url = "http://www.google.com")
            .Build();

            SiteMinder = RuleMinder.Create()
                .WithSslEnabled()
                .WithNoSpam(5, TimeSpan.FromHours(1))
                .AddRule<CreateRule<UrlIsValidRule, UrlRequest>, UrlIsValidRule, UrlRequest>(x =>
                    urlValid);
           
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            SiteMinder.VerifyAllRules();
          
            var ipaddress = GetIpAddress();

            var spamIpAddressCheck = new IpAddressRequest
            {
                IpAddress = ipaddress,
                CreatedUtcDateTime = DateTime.UtcNow,
            };

            var total = SiteMinder.GetRules<IpAddressRequest>().Sum(a => a.Items.Count(b => b.IpAddress == ipaddress));

            if (total > 2) // just for this demo start logging bad ips after 2 refreshes
            {
                spamIpAddressCheck.IsBadRequest = true;
                SiteMinder.VerifyRule(spamIpAddressCheck);
            }
            else
            {
                
                SiteMinder.VerifyRule(spamIpAddressCheck);
            }
            var msg = GetMessage(ipaddress);
            Response.Write(msg);
        }

        private static string GetIpAddress()
        {
            var wrapper = new HttpRequestWrapper(HttpContext.Current.Request);
            var ipaddress = RequestUtility.GetClientIpAddress(wrapper);
            return ipaddress;
        }

        private static string GetMessage(string ipaddress)
        {
            var total =
                SiteMinder.GetRules<IpAddressRequest>()
                    .Sum(a => a.Items.Count(b => b.IpAddress == ipaddress));

            var msg = string.Format("<h1>Website hit by IP {0} a total of {1} </h1>", ipaddress, total);
            return msg;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

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