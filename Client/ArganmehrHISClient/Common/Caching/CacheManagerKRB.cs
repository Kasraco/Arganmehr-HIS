
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Caching
{
    public class CacheManagerKRB<TCache> : ICacheManagerKRB where TCache : ICache
    {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly ICache _cache;

        private const string FakeNull = "__[NULL]__";

        public CacheManagerKRB(Func<Type, ICache> fn)
        {
            this._cache = fn(typeof(TCache));

        }
        public T Get<T>(string key, Func<T> acquirer, int? cacheTime = null)
        {
            if (_cache.Contains(key))
            {
                return GetExisting<T>(key);
            }

            using (EnterReadLock())
            {
                if (!_cache.Contains(key))
                {
                    var value = acquirer();
                    this.Set(key, value, cacheTime);

                    return value;
                }
            }

            return GetExisting<T>(key);
        }

        public void Set(string key, object value, int? cacheTime = null)
        {
            using (EnterWriteLock())
            {
                _cache.Set(key, value ?? FakeNull, cacheTime);
            }
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            using (EnterWriteLock())
            {
                _cache.Remove(key);
            }
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in _cache.Entries)
            {
                if (regex.IsMatch(item.Key))
                {
                    keysToRemove.Add(item.Key);
                }
            }

            using (EnterWriteLock())
            {
                foreach (string key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
        }

        public void Clear()
        {
            var keysToRemove = new List<string>();
            foreach (var item in _cache.Entries)
            {
                keysToRemove.Add(item.Key);
            }

            using (EnterWriteLock())
            {
                foreach (string key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
        }

        private IDisposable EnterReadLock()
        {
            return _cache.IsSingleton ? _rwLock.GetUpgradeableReadLock() : ActionDisposable.Empty;
        }

        public IDisposable EnterWriteLock()
        {
            return _cache.IsSingleton ? _rwLock.GetWriteLock() : ActionDisposable.Empty;
        }

        private T GetExisting<T>(string key)
        {
            var value = _cache.Get(key);

            if (value.Equals(FakeNull))
                return default(T);

            return (T)_cache.Get(key);
        }
    }
}
