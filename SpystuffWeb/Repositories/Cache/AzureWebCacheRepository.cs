using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Repositories.Config;

namespace Repositories.Cache
{
    public class AzureWebCacheRepository<T> : BaseCacheRepository<T, AzureCacheConfig>, IDataRepository<T> where T : new()
    {
        DataCache dc; 

        public AzureWebCacheRepository()
        {
            //DataCacheFactory dcf = new DataCacheFactory();
            //dc = dcf.GetDefaultCache(); 
            dc = new DataCache("default");             
        }

        public T Get(Models.Request request)
        {

            
            string key = base.GetKey(request);
            
            var obj = (T)dc.Get(key);

            return obj;             
        }

        public void Save(T obj, Models.Request request = null)
        {
            var key = GetKey(request, obj);
            dc.Put(key, obj, GetTTL()); 
        }

        public int Delete(Models.Request request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetList(Models.Request request)
        {
            var key = GetKey(request);
            return (IEnumerable<T>)dc.Get(key); 
        }

        public void SaveList(IEnumerable<T> list, Models.Request request = null)
        {
            var key = GetKey(request);
            dc.Put(key, list, GetTTL());                    
        }
    }
}
