using System;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressRequest : IRuleRequest
    {
        public IpAddressRequest()
        {
            this.Id = Guid.NewGuid();
            this.CreatedUtcDateTime = DateTime.UtcNow;
        }

        public string IpAddress { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
        public bool IsBadRequest { get; set; }
    }
}
