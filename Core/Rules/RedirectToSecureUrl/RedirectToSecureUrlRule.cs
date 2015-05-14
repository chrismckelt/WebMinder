using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.RedirectToSecureUrl
{
    public class SecureUrlRule : SimpleRuleSetHandler<UrlRequest>
    {
        public SecureUrlRule()
        {
            RuleSetName = "HTTP redirct to HTTPS";
            ErrorDescription = "Non secure request redirected to HTTPS";
            Rule = (request) => HttpContext.Current.Request.IsSecureConnection == false;
            InvalidAction =
                () =>
                    HttpContext.Current.Response.Redirect("https://" +
                                                          HttpContext.Current.Request.ServerVariables["HTTP_HOST"]
                                                          + HttpContext.Current.Request.RawUrl);
        }
    }
}
