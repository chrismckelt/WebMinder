using System;

namespace WebMinder.Core.Rules.UrlValid
{
    public class ValidUrlRequest :IRuleRequest
    {
        public Uri Endpoint { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}
