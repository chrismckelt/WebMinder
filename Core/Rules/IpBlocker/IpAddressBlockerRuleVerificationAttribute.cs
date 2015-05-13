using System;
using System.Web;

namespace WebMinder.Core.Rules.IpBlocker
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IpAddressBlockerRuleVerificationAttribute : Attribute
    {
        public IpAddressBlockerRuleVerificationAttribute()
        {
            string ip = RequestUtility.GetClientIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
            var request = new IpAddressRequest
            {
                IpAddress = ip,
                CreatedUtcDateTime = DateTime.UtcNow,
                IsBadRequest = false
            };
            RuleSetRunner.Instance.VerifyRule(request);
        }
    }
}
