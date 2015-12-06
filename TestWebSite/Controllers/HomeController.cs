using System.Web.Mvc;
using WebMinder.Core.Rules.ApiKey;

namespace TestWebSite.Controllers
{
    public class HomeController : Controller
    {
       // [ApiKeyRequired(HeaderKeyName = "About", HeaderApiToken = "123")]
        public ActionResult Index()
        {
            return View();
        }

      //  [ApiKeyRequired(HeaderKeyName = "About", HeaderApiToken = "123")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

     //   [ApiKeyRequired(HeaderKeyName = "Contact", HeaderApiToken = "123")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}