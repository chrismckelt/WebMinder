using System.Linq;

namespace WebMinder.Core.StorageProviders
{
    public interface IStorageProvider<T>
    {
        void Initialise(string[] args = null);
        IQueryable<T> Storage { get; set; } 
        void SaveStorage();
    }
}