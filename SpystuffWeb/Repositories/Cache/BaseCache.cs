using Common;
using Models;
using Repositories.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Cache
{
    public class BaseCacheRepository<T,TConfig> : BaseRepository<TConfig> where TConfig : CacheConfig
    {
        protected virtual string GetKey(Request request, T obj = default(T))
        {
            return GenerateKey(request, obj); 
        }

        protected virtual string GetListKey(Request request, T obj = default(T))
        {
            return GenerateKey(request, obj, "List");
        }

        private string GenerateKey(Request request, T obj = default(T), string nameSuffix = "")
        {
            if (request == null)
                request = new Request();

            if (request.Key != null)
                return request.Key;

            string hash = string.Empty;

            if (request.Parameters == null && obj != null)
            {
                //Create parameters based on Key attribute on Object						
                var propertyList = PropertyInfoHelper.GetProperties<T>();

                request.Parameters = new { Id = propertyList.KeyProperty.GetValue(obj, null) };
            }

            if (request.Parameters == null && obj == null)
            {
                request.Key = string.Format("{0}_{1}", typeof(T).Name, nameSuffix);
                return request.Key;
            }

            var parameterType = request.Parameters.GetType();
            if (typeof(IDictionary<string, object>).IsAssignableFrom(parameterType))
            {
                var p = (IDictionary<string, object>)request.Parameters;
                string strHash = string.Format(
                    "{0}_{1}",
                    string.Join("_", p.Keys.OrderBy(k => k)),
                    string.Join("_", p.Values.OrderBy(v => Convert.ToString(v))
                    ));

                hash = strHash.ToLower();
            }
            else
            {
                hash = request.Parameters.GetHashCode().ToString();
            }

            var type = typeof(T);
            string typeKey = type.Name;
            if (type.IsGenericType)
            {
                var types = type.GetGenericArguments();
                typeKey = "Generic[" + string.Join("_", types.Select(t => t.Name)) + "]";
            }

            request.Key = string.Format("{0}{1}_{2}", typeKey, nameSuffix, hash);

            return request.Key;


        }

        protected virtual TimeSpan GetTTL()
        {
            var ttl = TimeSpan.Parse(_config.DefaultTTL);

            var type = typeof(T);
            string typeName;
            if (type.IsGenericType)
            {
                typeName = type.GetGenericArguments().First().Name;
            }
            else
            {
                typeName = type.Name;
            }

            var typeItem = _config.Types[typeName];
            if (typeItem != null)
            {
                ttl = TimeSpan.Parse(typeItem.TTL);
            }

            return ttl;
        }
    }
}
