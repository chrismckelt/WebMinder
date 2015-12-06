using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMinder.Core;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.ApiKey;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.StorageProviders;

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
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            //var msg = string.Format("<h1>Website hit a total {0} </h1>", total);

            //if (total < 5)
            //{
            //    SiteMinder.RecordBadIpRequest();
            //}

            //Response.Write(msg);
            // Response.End();

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