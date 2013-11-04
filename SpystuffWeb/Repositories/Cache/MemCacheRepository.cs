using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Couchbase;
using System.Net;
using Couchbase.Configuration;
using Enyim.Caching.Memcached;
using Repositories.Config;
using Models;
 
namespace Repositories.Cache
{
    public class MemcacheRepository<T> : BaseCacheRepository<T, MemcacheConfig>, IDataRepository<T> where T : new() 
    {
        protected CouchbaseClient _client;

        protected override void LoadConfig(string configName)
        {
            base.LoadConfig(configName);

            _client = CouchbaseClientFactory.GetCouchbaseClient(_config);
        }

        public virtual T Get(Request request)
        {
            var key = GetKey(request);

            var obj = _client.Get<T>(key);

            return obj;
        }

        public virtual IEnumerable<T> GetList(Request request)
        {
            var key = GetListKey(request);

            return _client.Get<IEnumerable<T>>(key);
        }

        public virtual void Save(T obj, Request request)
        {
            var key = GetKey(request, obj);

            var storeResult = _client.Store(StoreMode.Set, key, obj, GetTTL());
        }

        public virtual void SaveList(IEnumerable<T> list, Request request = null)
        {
            var key = GetListKey(request);

            var storeResult = _client.Store(StoreMode.Set, key, list, GetTTL());
        }

        public virtual int Delete(Request request = null)
        {
            _client.Remove(GetKey(request));
            return 0;
        }
    }
}
