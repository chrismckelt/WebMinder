using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WebMinder.Core
{
    public class RequestAnalyserRuleSet<TRuleFor> : IRuleSetHandler<TRuleFor> where TRuleFor : IRuleRequest
    {

        private readonly IList<TRuleFor> _items;

        public RequestAnalyserRuleSet(Func<IList<TRuleFor>> storageMechanism = null)
        {
            StorageMechanism = storageMechanism;
            _items = storageMechanism != null ? StorageMechanism() : new List<TRuleFor>();
        }

        public string RuleSetName { get; set; }
        public string ErrorDescription { get; set; }
        public int? MaximumResultCount { get; set; }

        public IEnumerable<TRuleFor> Items
        {
            get { return _items; }
        }

        public Expression<Func<TRuleFor, bool>> Rule { get; set; }
        public Expression<Func<IEnumerable<TRuleFor>, bool>> AggregateRule { get; set; }

        public void Run(TRuleFor item)
        {
            _items.Add(item);

            VerifyMaximumCount();

            VerifySingleRule();

            VerifyAggregateRule();
        }

        public Func<IList<TRuleFor>> StorageMechanism { get; set; }

        private void VerifyMaximumCount()
        {
            if (!_items.Any()) return;
            if (MaximumResultCount.HasValue && _items.Count() >= MaximumResultCount)
            {
                throw new HttpException(403, ErrorDescription);
            }
        }

        private void VerifySingleRule()
        {
            if (Rule != null)
            {
                var invalid = _items
                    .AsQueryable()
                    .Where(Rule);

                if (invalid.Any()) throw new HttpException(403, ErrorDescription);
            }
        }

        private void VerifyAggregateRule()
        {
            if (AggregateRule != null)
            {
                var invalid = AggregateRule.Compile().Invoke(_items);

                if (invalid) throw new HttpException(403,ErrorDescription);
            }
        }

    }
}