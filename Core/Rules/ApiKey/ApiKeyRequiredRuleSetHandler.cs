using System.Threading.Tasks;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.ApiKey
{
    public class ApiKeyRequiredRuleSetHandler : SingleRuleSetHandler<ApiKeyRequiredRule>
    {

        public string HeaderKeyName { get; set; }

        public string HeaderKeyValue { get; set; }

        public ApiKeyRequiredRuleSetHandler()
        {
            RuleSetName = "API header key";

            ErrorDescription = "Header api key and token required";

            Rule = request => IsApiKeyInvalid();

            InvalidAction = () =>
            {
                var ex = new HttpException(403, $"{RuleSetName} Api key or token invalid: {RuleRequest.Id}");
                throw ex;
            };
        }

        private bool IsApiKeyInvalid()
        {
            var foundKey = RequestUtility.GetRequest().Headers.Get(HeaderKeyName);
            if (string.IsNullOrEmpty(foundKey))
            {
                LogWarn("key not found in configuration file");
                InvalidAction();
            }
            var token = RequestUtility.GetRequest().Headers.Get(HeaderKeyName);
            bool matched = token != null && HeaderKeyValue == token;

            if (!matched)
            {
                LogWarn($"Invalid token from client - {token}");
                InvalidAction();
            }

            return false;
        }
    }
}
