using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace WebMinder.Core.StorageProviders
{
    public class MemoryCacheStorageProvider<T> : IStorageProvider<T>
    {
        public string CacheName { get; set; }

        public void Initialise(string[] args)
        {
            CacheName = args.First();
            Items = MemoryCache.Default.Get(CacheName) as IQueryable<T>;
            if (Items == null)
            {
                MemoryCache.Default.Add(CacheName, new List<T>().AsQueryable(), null);
                Items = MemoryCache.Default.Get(CacheName) as IQueryable<T>;
            }
        }

        public IQueryable<T> Items { get; set; }

        public void SaveStorage()
        {
            MemoryCache.Default.Add(CacheName, Items, null);
        }
    }
}