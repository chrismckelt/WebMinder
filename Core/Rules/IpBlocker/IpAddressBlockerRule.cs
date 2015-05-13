using System;
using System.Linq;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.IpBlocker
{
    public class IpAddressBlockerRule : AggregateRuleSetHandler<IpAddressRequest>
    {
        public TimeSpan? Duration { get; set; }
        public int? MaxAttemptsWithinDuration { get; set; }

        public IpAddressBlockerRule()
        {
            SetDefaults();

            RuleSetName = string.Format("Block IP Addresses with more than {0} unsuccessful tries over a {1} hour period", MaxAttemptsWithinDuration, Duration.GetValueOrDefault());

            ErrorDescription =
                "If an IP Address has been used in an unsuccessful search more than 5 times in a 24 hour period, then return an unsuccessful search result (even if the search result should be a success).";

            AggregateFilter = (requestCollection, request) => requestCollection.Where(x => x.IpAddress == request.IpAddress && request.IsBadRequest) ;

            AggregateRule =
               ipAddressRequests =>
                   (from ipRequest in
                        ipAddressRequests.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(Duration.Value.Ticks))
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
                var ex = new HttpException(403, string.Format("{0}  Bad IP Address: {1}", RuleSetName, RuleRequest.IpAddress));
                throw ex;
            };

            
        }

        private void SetDefaults()
        {
            if (!Duration.HasValue) Duration = TimeSpan.FromDays(-1);
            if (!MaxAttemptsWithinDuration.HasValue) MaxAttemptsWithinDuration = 5;
        }
    }
}
