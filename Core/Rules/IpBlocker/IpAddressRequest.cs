using System;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressRequest : IRuleRequest
    {
        public string IpAddress { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }

        public bool IsBadRequest { get; set; }
    }
}
