using System.Net.Http;
using System.Web;

namespace WebMinder.Core.Rules
{
    public class UrlRequest : RuleRequest
    {
        public string Url { get; set; }

        public static UrlRequest GetCurrentUrl()
        {
            return new UrlRequest() {Url = HttpContext.Current.Request.RawUrl};
        }
    }
}
