using System;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressRequest : RuleRequest
    {
        public string IpAddress { get; set; }
        public bool IsBadRequest { get; set; }

        public static IpAddressRequest GetCurrentIpAddress(bool? recordBadIp = null)
        {
            var request = RequestUtility.GetRequest();
            return new IpAddressRequest()
            {
                IpAddress = RequestUtility.GetClientIpAddress(request),
                CreatedUtcDateTime = DateTime.UtcNow,
                IsBadRequest = recordBadIp.GetValueOrDefault()
            };
        }
    }

 
}
