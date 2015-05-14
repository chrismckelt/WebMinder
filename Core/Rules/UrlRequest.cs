using System;

namespace WebMinder.Core.Rules
{
    public class UrlRequest : IRuleRequest
    {
        public string Url { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}
