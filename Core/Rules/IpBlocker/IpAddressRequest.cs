using System;
using System.Web;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressRequest : RuleRequest
    {
        public string IpAddress { get; set; }
        public bool IsBadRequest { get; set; }

        public static IpAddressRequest GetCurrentIpAddress(bool? recordBadIp = null)
        {
            if (HttpContext.Current == null) return null;
            var request = new HttpRequestWrapper(HttpContext.Current.Request);
            return new IpAddressRequest()
            {
                IpAddress = RequestUtility.GetClientIpAddress(request),
                CreatedUtcDateTime = DateTime.UtcNow,
                IsBadRequest = recordBadIp.GetValueOrDefault()
            };
        }
    }
}
