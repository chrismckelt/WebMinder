using System;
using System.Linq;
using System.Web;

namespace WebMinder.Core.Rules
{
    public class IpAddressBlockerRule : RuleSetHandler<IpAddressRequest>
    {
        public TimeSpan? Duration { get; set; }
        public int? MaxAttemptsWithinDuration { get; set; }

        public IpAddressBlockerRule()
        {
            RuleSetName = "Block IP Addresses with 5 or more unsuccessful tries over a 24 hour period";

            ErrorDescription =
                "If an IP Address has been used in an unsuccessful search more than 5 times in a 24 hour period, then return an unsuccessful search result (even if the search result should be a success).";

            if (!Duration.HasValue) Duration = TimeSpan.FromDays(-1);
            if (!MaxAttemptsWithinDuration.HasValue) MaxAttemptsWithinDuration = 5;

            AggregateFilter = (data, item) => data.Where(x => x.IpAddress == item.IpAddress);

            AggregateRule =
                data => (from x in data.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(Duration.Value.Ticks))
                         let observableSet = data.GroupBy(b => b.IpAddress, b => b.IpAddress, (key, c) => new
                         {
                             IpAddress = key,
                             Total = c.ToList()
                         })
                         from y in observableSet
                         where y.Total.Count > MaxAttemptsWithinDuration
                         select new IpAddressRequest
                         {
                             IpAddress = x.IpAddress
                         }).Any();

            InvalidAction = () =>
            {
                var ex = new HttpException(403, string.Format("{0}  Bad IP Address: {1}", RuleSetName, RuleRequest.IpAddress));
                throw ex;
            };
        }
    }
}
