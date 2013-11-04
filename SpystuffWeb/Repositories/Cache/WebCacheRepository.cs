using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Repositories.Config;

namespace Repositories.Cache
{
    public class WebCacheRepository<T> : BaseCacheRepository<T, CacheConfig>, IDataRepository<T> where T : new()
    {
        private System.Web.Caching.Cache _cache;

        public WebCacheRepository()
        {
            _cache = System.Web.HttpRuntime.Cache;
        }

        public T Get(Models.Request request)
        {            
            string key = base.GetKey(request);
            
            var obj = (T)_cache.Get(key);

            return obj;             
        }

        public void Save(T obj, Models.Request request = null)
        {
            var key = GetKey(request, obj);
            _cache.Insert(
                key,
                obj,
                null,
                DateTime.UtcNow + GetTTL(),
                System.Web.Caching.Cache.NoSlidingExpiration
                );            
        }

        public int Delete(Models.Request request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetList(Models.Request request)
        {
            var key = GetKey(request);
            return (IEnumerable<T>)_cache.Get(key); 
        }

        public void SaveList(IEnumerable<T> list, Models.Request request = null)
        {
            var key = GetKey(request);
            _cache.Insert(
                key,
                list,
                null,
                DateTime.UtcNow + GetTTL(),
                System.Web.Caching.Cache.NoSlidingExpiration
                );
        }
    }
}
