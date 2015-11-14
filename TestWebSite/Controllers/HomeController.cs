using System.Web.Mvc;
using WebMinder.Core.Rules.ApiKey;

namespace TestWebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ApiKey(HeaderApiKeyName = "About", HeaderApiToken = "123")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ApiKey(HeaderApiKeyName = "Contact", HeaderApiToken = "123")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}