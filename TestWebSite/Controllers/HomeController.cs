using System.Web.Mvc;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules.ApiKey;

namespace TestWebSite.Controllers
{
    public class HomeController : Controller
    {
       [ApiKeyRequired]
        public ActionResult Index()
        {
            return View();
        }

        [ApiKeyRequired]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ApiKeyRequired]
        public ActionResult Contact()
        {
            SiteMinder.ValidateApiKey();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}