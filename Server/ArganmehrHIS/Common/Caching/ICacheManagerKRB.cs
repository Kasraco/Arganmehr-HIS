using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Caching
{
    public interface ICacheManagerKRB
    {
        T Get<T>(string key, Func<T> acquirer, int? cacheTime = null);
        void Set(string key, object value, int? cacheTime = null);
        bool Contains(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);
        void Clear();
        IDisposable EnterWriteLock();


    }
}
