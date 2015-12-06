using System;
using System.Linq;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressBlockerRuleSetHandler : AggregateRuleSetHandler<IpAddressRequest>
    {
        public TimeSpan? Duration { get; set; }
        public int? MaxAttemptsWithinDuration { get; set; }

        public IpAddressBlockerRuleSetHandler()
        {
            SetDefaults();

            RuleSetName =
                $"Block IP Addresses with more than {MaxAttemptsWithinDuration} unsuccessful tries over a {Duration.GetValueOrDefault()} hour period";

            ErrorDescription =
                "If an IP Address has been used in an unsuccessful search more than 5 times in a 24 hour period, then return an unsuccessful search result (even if the search result should be a success).";

            AggregateFilter = (requestCollection, request) => requestCollection.Where(x => x.IpAddress == request.IpAddress && request.IsBadRequest) ;

            AggregateRule =
               ipAddressRequests =>
                   (from ipRequest in
                        ipAddressRequests.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(GetTicks()))
                    let totalHitsForIpAddress = ipAddressRequests.GroupBy(b => b.IpAddress, b => b.IpAddress, (key, c) => new
                    {
                        IpAddress = key,
                        Total = c.ToList()
                    })
                    from targetIp in totalHitsForIpAddress
                    where targetIp.Total.Count > MaxAttemptsWithinDuration
                    select ipRequest).Any();

            InvalidAction = () =>
            {
                var ex = new HttpException(403, $"{RuleSetName}  Bad IP Address: {RuleRequest.IpAddress}");
                throw ex;
            };
        }

        private long GetTicks()
        {
            var ticks = Duration.GetValueOrDefault().Ticks;
            if (ticks > 0) return ticks*-1;
            return ticks;
        }

        private void SetDefaults()
        {
            if (!Duration.HasValue) Duration = TimeSpan.FromDays(-1);
            if (!MaxAttemptsWithinDuration.HasValue) MaxAttemptsWithinDuration = 5;
        }
    }
}
