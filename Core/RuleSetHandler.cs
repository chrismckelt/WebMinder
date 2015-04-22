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
        private T _ruleRequest;

        public RuleSetHandler(Func<IList<T>> storageMechanism = null)
        {
            StorageMechanism = storageMechanism;
            _items = new List<IRuleRequest>();
            RuleSetName = RuleType.Name + " ruleset";
            ErrorDescription = RuleType.Name + " was invalid";
            InvalidAction = () => { throw new HttpException(403, ErrorDescription); };

            if (storageMechanism == null) return;
            var storage = storageMechanism() ?? new List<T>();
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

        public T RuleRequest
        {
            get { return _ruleRequest; }
        }

        public Expression<Func<IEnumerable<T>, T, IEnumerable<T>>> AggregateFilter { get; set; }
        public Expression<Func<IEnumerable<T>, bool>> AggregateRule { get; set; }

        public void Run(IRuleRequest item, bool addRequestToItemsCollection = true)
        {
            _ruleRequest = (T)item;
            if (addRequestToItemsCollection)
                _items.Add((T) item);

            VerifyMaximumCount();

            VerifySingleRule();

            VerifyAggregateRule();
        }

        public Func<IList<T>> StorageMechanism { get; set; }
        public Action InvalidAction { get; set; }

        private void VerifyMaximumCount()
        {
            if (!_items.Any()) return;
            if (MaximumResultCount.HasValue && _items.Count() >= MaximumResultCount)
            {
                InvalidAction();
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

                if (invalid.Any()) InvalidAction();
            }
        }

        private void VerifyAggregateRule()
        {
            if (AggregateRule == null) return;
            IEnumerable<T> filtered;
            if (AggregateFilter != null)
            {
                filtered = AggregateFilter.Compile().Invoke(_items.Cast<T>(),_ruleRequest);
            }
            else
            {
                filtered = _items.Cast<T>();
            }

            var invalid = AggregateRule.Compile().Invoke(filtered);
            if (invalid) InvalidAction();
        }
    }
}