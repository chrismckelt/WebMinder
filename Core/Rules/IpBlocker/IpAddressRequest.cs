namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressRequest : RuleRequest
    {
        public string IpAddress { get; set; }
        public bool IsBadRequest { get; set; }
    }
}
