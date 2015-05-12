using System;

namespace WebMinder.Core.Rules
{
    public class IpAddressRequest : IRuleRequest
    {
        public string IpAddress { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}
