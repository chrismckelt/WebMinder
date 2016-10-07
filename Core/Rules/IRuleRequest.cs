using System;

namespace WebMinder.Core.Rules
{
    [Obsolete("use IValidatableRule")]
    public interface IRuleRequest
    {
        Guid Id { get; set; }
        DateTime CreatedUtcDateTime { get; set; }
    }
}
