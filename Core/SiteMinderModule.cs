using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebMinder.Core
{
    public class SiteMinderModule : IHttpModule
    {
        public delegate void WebMinderRuleRequestReportedEventHandler(object sender, EventArgs e);
        public event WebMinderRuleRequestReportedEventHandler RuleRequestReported;

        public void Init(HttpApplication app)
        {
            app.BeginRequest += AppBeginRequest;
            app.PostMapRequestHandler += AppPostMapRequestHandler;
            app.EndRequest += AppEndRequest;
        }

        void AppBeginRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            var context = new HttpContextWrapper(app.Context);
            
            OnRuleRequestReported(eventArgs);

        }

        void AppPostMapRequestHandler(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = new HttpContextWrapper(app.Context);

        }

        void AppEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = new HttpContextWrapper(app.Context);
        }

        protected virtual void OnRuleRequestReported(EventArgs e)
        {
            if (RuleRequestReported != null)
            {
                //Invokes the delegates.
                RuleRequestReported(this, e);
            }
        }

        public void Dispose()
        {

        }
    }
}
