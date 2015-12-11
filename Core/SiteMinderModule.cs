using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
              // .WithSslEnabled()
               .WithApiKeyValidation()
               .WithIpWhitelist()
               .WithNoSpam(500, TimeSpan.FromHours(1))
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

            // ignore files such as jpegs
            string ext = Path.GetExtension(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath);

            if (!string.IsNullOrWhiteSpace(ext) || Utilities.Util.GetFileExtensions().Any(a => a == ext.ToLowerInvariant()))
            {
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
            Trace.Write(e);
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
