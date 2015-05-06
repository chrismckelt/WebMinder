using System;
using System.Linq;
using System.Web;

namespace WebMinder.Core.Rules
{
    public enum ValidUrlCircuitBreakerStatus
    {
        Closed,
        Open,
        HalfOpen
    }

    public class ValidUrlCircuitBreakerRule : RuleSetHandler<ValidUrlRequest>
    {
        public ValidUrlCircuitBreakerStatus Status { get; private set; }
        public TimeSpan? Duration { get; set; }
        public int? MaxAttemptsWithinDuration { get; set; }

        public ValidUrlCircuitBreakerRule()
        {
            RuleSetName = "Yodlee Bank details circuit breaker";

            ErrorDescription = "Unable to retrieve Yodlee bank statements. Skip to Verification screen";

            SetDefaultValues();

            AggregateFilter = (data, item) => data.Where(x => x.Endpoint == item.Endpoint);

            AggregateRule =
                data => (from x in data.Where(a => a.CreatedUtcDateTime >= DateTime.UtcNow.AddTicks(Duration.Value.Ticks))
                         let observableSet = data.GroupBy(b => b.Endpoint, b => b.Endpoint, (key, c) => new
                         {
                             IpAddress = key,
                             Total = c.ToList()
                         })
                         from y in observableSet
                         where y.Total.Count > MaxAttemptsWithinDuration
                         select x).Any();

            InvalidAction = () =>
            {
                var ex = new HttpException(403, string.Format("{0}  Endpoint unavailable: {1}", RuleSetName, RuleRequest.Endpoint));
                throw ex;
            };
        }

        private void SetDefaultValues()
        {
            if (!Duration.HasValue) Duration = TimeSpan.FromDays(-1);
            if (!MaxAttemptsWithinDuration.HasValue) MaxAttemptsWithinDuration = 5;
        }
    }
}
