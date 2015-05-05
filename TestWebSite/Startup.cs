using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestWebSite.Startup))]
namespace TestWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
