using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using WebMinder.Core.Builders;

namespace WebMinder.Core
{
    public class SiteMinderModule : IHttpModule
    {
        public delegate void WebMinderRuleRequestReportedEventHandler(object sender, SiteMinderFailuresEventArgs e);
        public event WebMinderRuleRequestReportedEventHandler RuleRequestReported;
        public static SiteMinder SiteMinder { get; set; }

        public void Init(HttpApplication app)
        {

            app.BeginRequest += AppBeginRequest;
            SiteMinder = SiteMinder.Create()
               .WithSslEnabled()
               .WithApiKeyValidation()
               .WithIpWhitelist()
               .WithNoSpam(50, TimeSpan.FromHours(1))
               ;

            SiteMinder.Initialise();
        }

        void AppBeginRequest(object sender, EventArgs eventArgs)
        {
            if (HttpContext.Current.Response.StatusCode != 200)
            {
                Trace.Write("SiteMinder rules not running due to HTTP Status code: " + HttpContext.Current.Response.StatusCode);
                return;
            }
            SiteMinder.ValidateWhiteList();
            SiteMinder.EnforceSsl();
            if (SiteMinder.AllRulesValid()) return;
            var args = new SiteMinderFailuresEventArgs { Failures = SiteMinder.Failures };
            OnRuleRequestReported(args);
        }

        protected virtual void OnRuleRequestReported(SiteMinderFailuresEventArgs e)
        {
            RuleRequestReported?.Invoke(this, e);
        }

        public void Dispose()
        {

        }
    }

    public class SiteMinderFailuresEventArgs : EventArgs
    {
        public List<Exception> Failures { get; set; }
    }
}
