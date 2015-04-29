using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Web;

namespace WebMinder.Core
{
    public class RuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        private T _ruleRequest;
        private Func<IList<T>> _storageMechanism;
        private Action<string, string> _logger;

        public RuleSetHandler()
        {
            RuleSetName = RuleType.Name + " ruleset";
            ErrorDescription = RuleType.Name + " was invalid";

            if (InvalidAction == null)
            {
                InvalidAction = () => { throw new HttpException(403, ErrorDescription); };
            }

            if (_logger == null)
            {
                _logger = (a,b) => Console.WriteLine(a + " :: " + b);
            }
        }

        public string RuleSetName { get; set; }

        public string ErrorDescription { get; set; }

        public Type RuleType
        {
            get { return typeof (T); }
        }

        public int? MaximumResultCount { get; set; }

        public IEnumerable<T> Items
        {
            get { return StorageMechanism().AsEnumerable(); }
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
            {                
                StorageMechanism().Add((T)item);
                _logger("INFO", "Added item to collection. Count = " + StorageMechanism().Count);
            }
            
            _logger("DEBUG", "Verify Maximum Count Rule");
            VerifyMaximumCount();
            _logger("DEBUG", "Verify Single Rule");
            VerifySingleRule();
            _logger("DEBUG", "Verify Aggregate Rule");
            VerifyAggregateRule();

            _logger("INFO", "Request passed rule : " + RuleType.Name);
        }

        public Func<IList<T>> StorageMechanism
        {
            get
            {
                if (_storageMechanism == null)
                {
                    _logger("WARNING","No storage mechanism specified. Default to Cache storage");
                    UseCacheStorage();
                }
                return _storageMechanism;
            }
            set { _storageMechanism = value; }
        }

        public Action InvalidAction { get; set; }

        public void UseCacheStorage()
        {
            var cache = MemoryCache.Default.Get(RuleType.Name) as IList<T>;
            if (cache == null)
            {
                MemoryCache.Default.Add(RuleType.Name, new List<T>(),null);
                cache = MemoryCache.Default.Get(RuleType.Name) as IList<T>;
            }
            _storageMechanism = () => cache;
        }

        public void AddExistingItems(IEnumerable<T> existingItems)
        {
            var enumerable = existingItems as T[] ?? existingItems.ToArray();
            if (existingItems != null && enumerable.Any())
            {
                _logger("DEBUG", "AddExistingItems: " + enumerable.Count());
                enumerable.ToList().ForEach(StorageMechanism().Add);
            }
        }

        public void Log(Action<string, string> logger)
        {
            _logger = logger;
        }

        private void VerifyMaximumCount()
        {
            if (!StorageMechanism().Any()) return;
            if (MaximumResultCount.HasValue && StorageMechanism().Count() >= MaximumResultCount)
            {
                _logger("WARN", "Rule Failed VerifyMaximumCount");
                InvalidAction();
            }
        }

        private void VerifySingleRule()
        {
            if (Rule != null)
            {
                var invalid = StorageMechanism()
                    .AsQueryable()
                    .Cast<T>()
                    .Where(Rule);

                if (invalid.Any())
                {
                    _logger("WARN", "Rule Failed VerifySingleRule");
                    InvalidAction();
                }
            }
        }

        private void VerifyAggregateRule()
        {
            if (AggregateRule == null) return;
            IEnumerable<T> filtered;
            if (AggregateFilter != null)
            {
                filtered = AggregateFilter.Compile().Invoke(StorageMechanism().Cast<T>(),_ruleRequest);
            }
            else
            {
                filtered = StorageMechanism().Cast<T>();
            }

            var invalid = AggregateRule.Compile().Invoke(filtered);
            if (invalid)
            {
                _logger("WARN", "Rule Failed VerifyAggregateRule");
                InvalidAction();
            }
        }
    }
}