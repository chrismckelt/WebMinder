using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Rules;
using WebMinder.Core.Runners;
using WebMinder.Core.StorageProviders;

namespace WebMinder.Core.Handlers
{
    public interface IRuleSetHandler<T> : IRuleRunner where T : IRuleRequest, new()
    {
        string RuleSetName { get; set; }

        string ErrorDescription { get; set; }

        Type RuleType { get; }

        IEnumerable<T> Items { get; }

        T RuleRequest { get; }

        IStorageProvider<T> StorageMechanism { get; set; }

        Action InvalidAction { get; set; }

        void UseMemoryCacheStorage(string cacheName = null);

        void AddExistingItems(IEnumerable<T> existingItems);

        void Log(Action<string, string> logger);

        bool UpdateRuleCollectionOnSuccess { get; set; }
    }
}
