using System.Collections.Generic;
using System.Web;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core.Rules.IpWhitelist
{
    public class IpWhitelistRuleSetHandler : SingleRuleSetHandler<IpAddressRequest>
    {
        public IDictionary<string ,string> ValidIpRanges { get; set; } 

        public IpWhitelistRuleSetHandler()
        {

            RuleSetName =
                $"IP Addresses from white list only";

            ErrorDescription =
                "IP address not allowed";

            Rule = (request) => InvalidIp();

           InvalidAction = () =>
            {
                var ex = new HttpException(403, $"{RuleSetName}  Bad IP Address: {RuleRequest.IpAddress}");
                throw ex;
            };
        }

        private bool InvalidIp()
        {
            var ip = RequestUtility.GetCurrentIpAddress();

            foreach (var validIpRange in ValidIpRanges)
            {
                if (RequestUtility.IsInRange(validIpRange.Key, validIpRange.Value, ip))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
