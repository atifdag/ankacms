using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Core.Caching
{

    /// <inheritdoc />
    /// <summary>
    /// RAM önbellekme işlemlerini yapan sınıf
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly int _cacheTime;

        private readonly IMemoryCache _cache;
        private const string MemoryCacheMainKey = "AnkaCMSWebApiCacheMainKey";
        public MemoryCacheService(IMemoryCache cache, int cacheTime)
        {
            _cache = cache;
            _cacheTime = cacheTime;
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out var o);

        }

        public void Add(string key, object value)
        {
            // Değer boş ise ekleme yapılmaz
            if (value == null)
            {
                return;
            }

            // Anahtar daha öce kullanıldı ise ekleme yapılmaz
            if (Exists(key))
            {
                return;
            }

            // Kayıt eklenir
            _cache.Set(key, value, DateTimeOffset.Now.AddHours(_cacheTime));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public List<string> GetAllKeyList()
        {
            var list = new List<string>();
            if (Exists(MemoryCacheMainKey))
            {
                list = Get<List<string>>(MemoryCacheMainKey);
            }
            return list;
        }

        public void AddToKeyList(string key)
        {
            var keyList = GetAllKeyList();

            if (!keyList.Contains(key))
            {
                keyList.Add(key);
            }
            Remove(MemoryCacheMainKey);
            Add(MemoryCacheMainKey, keyList);
        }

        public void RemoveFromKeyList(string key)
        {
            var keyList = GetAllKeyList();

            if (keyList.Contains(key))
            {
                keyList.Remove(key);
            }
            Remove(MemoryCacheMainKey);
            Add(MemoryCacheMainKey, keyList);
        }

        public void CleanKeyList()
        {
            foreach (var key in GetAllKeyList())
            {
                Remove(key);
            }
            Remove(MemoryCacheMainKey);
        }
    }
}
