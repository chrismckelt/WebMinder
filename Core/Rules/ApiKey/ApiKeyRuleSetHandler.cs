using System.Threading.Tasks;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyRuleSetHandler : SingleRuleSetHandler<ApiKeyRuleRequest>
    {
        public ApiKeyRuleSetHandler()
        {
            RuleSetName = "API header key";

            ErrorDescription = "Header api key and token required";

            Rule = request => ValidateApiKey();

            InvalidAction = () =>
            {
                var ex = new HttpException(403, $"{RuleSetName} Api key or token invalid: {RuleRequest.Id}");
                throw ex;
            };
        }

        private bool ValidateApiKey()
        {
            var foundKey = RequestUtility.GetRequest().Headers.Get(RuleRequest.HeaderApiKeyName);
            if (string.IsNullOrEmpty(foundKey)) return false;
            var token = RequestUtility.GetRequest().Headers.GetValues(foundKey);
            return  token != null && RuleRequest.HeaderToken == token[0];
        }
    }
}
