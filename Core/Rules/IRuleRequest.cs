using System;

namespace WebMinder.Core.Rules
{
    public interface IRuleRequest
    {
        DateTime CreatedUtcDateTime { get; set; }
    }
}
