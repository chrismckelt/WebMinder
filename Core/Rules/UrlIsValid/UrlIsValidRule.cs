using System.Threading.Tasks;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.UrlIsValid
{
    public class UrlIsValidRule : SimpleRuleSetHandler<UrlRequest>
    {
        public UrlIsValidRule()
        {
            RuleSetName = "URL is valid";

            ErrorDescription = "Invalid url";

            Rule = request =>  CheckUrl().Result;

            InvalidAction = () =>
            {
                var ex = new HttpException(403, string.Format("{0}  Url unavailable: {1}", RuleSetName, RuleRequest.Url));
                throw ex;
            };
        }

        private async Task<bool> CheckUrl()
        {
            return await RequestUtility.UrlIsValid(RuleRequest.Url, this._logger); ;//TOOD async all the way through the request
        }
    }
}
