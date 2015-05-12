using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace WebMinder.Core.Handlers
{
    public abstract class RuleSetHandlerBase<T> : IRuleRunner where T : IRuleRequest, new()
    {
        protected T _ruleRequest;
        protected Func<IList<T>> _storageMechanism;

        protected Action<string, string> _logger;
        public string RuleSetName { get; set; }
        public string ErrorDescription { get; set; }

        public T RuleRequest
        {
            get { return _ruleRequest; }
        }

        public Type RuleType
        {
            get { return typeof (T); }
        }

        public IEnumerable<T> Items
        {
            get { return StorageMechanism().AsEnumerable(); }
        }

        public Action InvalidAction { get; set; }

        public Func<IList<T>> StorageMechanism
        {
            get
            {
                if (_storageMechanism == null)
                {
                    _logger("WARNING", "No storage mechanism specified. Default to Cache storage");
                    UseCacheStorage();
                }
                return _storageMechanism;
            }
            set { _storageMechanism = value; }
        }

        protected RuleSetHandlerBase()
        {
            RuleSetName = RuleType.Name + " ruleset";
            ErrorDescription = RuleType.Name + " was invalid";

            if (InvalidAction == null)
            {
                InvalidAction = () => { throw new HttpException(403, ErrorDescription); };
            }

            if (_logger == null)
            {
                _logger = (a, b) => Console.WriteLine(a + " :: " + b);
            }

            UpdateRuleCollectionOnSuccess = true;
        }

        public void UseCacheStorage(string cacheName = null)
        {
            if (string.IsNullOrEmpty(cacheName))
            {
                cacheName = RuleType.Name;
            }

            var cache = MemoryCache.Default.Get(cacheName) as IList<T>;
            if (cache == null)
            {
                MemoryCache.Default.Add(cacheName, new List<T>(), null);
                cache = MemoryCache.Default.Get(cacheName) as IList<T>;
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

        protected void RecordRequest(IRuleRequest request)
        {
           if (UpdateRuleCollectionOnSuccess)
           {
               if (!StorageMechanism().Contains((T)request))
                   StorageMechanism().Add((T)request);
           }
        }

        public virtual void VerifyRule(IRuleRequest request = null) 
        {
            _ruleRequest = (T)request;
            RecordRequest(request);
        }

        public bool UpdateRuleCollectionOnSuccess {get;set;}

    }
}