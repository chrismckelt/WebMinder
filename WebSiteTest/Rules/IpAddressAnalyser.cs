using System;
using WebMinder.Core;

namespace WebMinder.WebSiteTest.Rules
{
    public class IpAddressAnalyser : IRuleRequest
    {
        public string IpAddress { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}