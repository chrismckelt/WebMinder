using System;

namespace WebMinder.Core.Rules
{
    public interface IRuleRequest
    {
        Guid Id { get; set; }
        DateTime CreatedUtcDateTime { get; set; }
    }
}
