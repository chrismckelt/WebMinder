using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WebMinder.Core
{
    public class RuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        private readonly IList<IRuleRequest> _items;

        public RuleSetHandler(Func<IList<T>> storageMechanism = null)
        {
           // StorageMechanism = storageMechanism
            _items = new List<IRuleRequest>();
            var storage = storageMechanism();
            foreach (var stored in storage)
            {
                _items.Add(stored);
            }
        }

        public string RuleSetName { get; set; }
        public string ErrorDescription { get; set; }

        public Type RuleType
        {
            get { return typeof (T); }
        }

        public int? MaximumResultCount { get; set; }

        public IEnumerable<IRuleRequest> Items
        {
            get { return _items; }
        }

        public Expression<Func<T, bool>> Rule { get; set; }
        public Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }

        public void Run(IRuleRequest item)
        {
            _items.Add((T)item);

            VerifyMaximumCount();

            VerifySingleRule();

            VerifyAggregateRule();
        }

        public Func<IList<IRuleRequest>> StorageMechanism { get; set; }

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
                    .Cast<T>()
                    .Where(Rule);

                if (invalid.Any()) throw new HttpException(403, ErrorDescription);
            }
        }

        private void VerifyAggregateRule()
        {
            if (AggregateRule != null)
            {
                var invalid = AggregateRule.Compile().Invoke(_items.Cast<T>());

                if (invalid) throw new HttpException(403,ErrorDescription);
            }
        }
    }
}