using System;

namespace WebMinder.Core
{
    public interface IRuleRequest
    {
        DateTime CreatedUtcDateTime { get; set; }
    }
}
