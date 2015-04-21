using System;
using Charon.Core;

namespace Charon.WebSiteTest.Rules
{
    public class IpAddressAnalyser : IRuleRequest
    {
        public string IpAddress { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }
    }
}