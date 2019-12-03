using AnkaCMS.Core.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnkaCMS.Core.Caching
{
    public class RedisCacheService : ICacheService
    {
        private static int _cacheTime;
        private static Lazy<ConnectionMultiplexer> _lazy;
        private const string MemoryCacheMainKey = "AnkaCMSWebApiCacheMainKey";

        public RedisCacheService(string host, int port, int cacheTime)
        {
            _cacheTime = cacheTime;


            _lazy = new Lazy<ConnectionMultiplexer>(() =>
            {
                var conf = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    ConnectTimeout = 100,
                };

                conf.EndPoints.Add(host, port);

                return ConnectionMultiplexer.Connect(conf.ToString());
            });
        }

        private static ConnectionMultiplexer Connection => _lazy.Value;
        private static IDatabase Cache => Connection.GetDatabase();

        public object Get(string key)
        {
            return Cache.StringGet(key);
        }

        public T Get<T>(string key)
        {
            T value;
            if (Cache.StringGet(key).HasValue)
            {
                var redisValue = Cache.StringGet(key);
                var sData = (string)redisValue;
                var binaryDeserilized = sData.DeserializeFromString<string>();
                value = JsonConvert.DeserializeObject<T>(binaryDeserilized);

            }
            else
            {
                value = default(T);
            }
            return value;
        }

        public bool Exists(string key)
        {
            return Cache.StringGet(key).HasValue;
        }

        public void Add(string key, object value)
        {
            if (Get<object>(key) != null)
            {
                return;
            }

            var jsonSerialized = JsonConvert.SerializeObject(value);
            var binarySerilized = jsonSerialized.SerializeToString();
            Cache.StringSet(key, binarySerilized, TimeSpan.FromHours(_cacheTime));
        }

        public void Remove(string key)
        {
            Cache.KeyDelete(key);
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
