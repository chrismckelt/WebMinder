using System;

namespace WebMinder.Core.Rules
{
    public class RuleRequest : IRuleRequest
    {
        public RuleRequest()
        {
            this.Id = Guid.NewGuid();
            this.CreatedUtcDateTime = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}