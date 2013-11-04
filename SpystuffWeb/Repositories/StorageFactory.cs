using Repositories.Cache;
using Repositories.Config;
using Repositories.RavenDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public static class StorageFactory<T> where T: new()
    {
        private static readonly Dictionary<string, IStorage<T>> Warehouse = new Dictionary<string, IStorage<T>>();

        public static IStorage<T> GetStorage() 
        {
            IStorage<T> res = null;
            var typeName = typeof(T).Name;

            //Check if we have a store in the warehouse
            if (Warehouse.ContainsKey(typeof(T).Name))
                res = (IStorage<T>)Warehouse[typeof(T).Name];

            //If not then create a new one
            if (res == null)
            {
                var config = Global.GetConfig<StorageConfig>("StorageConfig");

                var typeStorageConfig = config.Types[typeName];

                var storageRepos = new List<IDataRepository<T>>();

                if (typeStorageConfig == null)
                {
                    typeStorageConfig = config.Types["Default"];
                }

                foreach (ReproItem repro in typeStorageConfig.Repros)
                {
                    IDataRepository<T> storageRepro = null;
                    switch (repro.Name)
                    {
                        case "WebCache":
                            storageRepro = new WebCacheRepository<T>();
                            break;
                        case "AzureCache":
                            storageRepro = new AzureWebCacheRepository<T>();
                            break;                      
                        //case "EditModelMemCache":
                        //    storageRepro = new EditModelMemcacheRepository<T>();
                        //    break;
                        case "MemCache":
                            storageRepro = new MemcacheRepository<T>();
                            break;
                        case "RavenDB":
                            storageRepro = new RavenDatabaseRepository<T>();
                            break;
                        case "Database":
                            storageRepro = DatabaseRepositoryFactory.GetRep<T>();
                            break;
                    }

                    storageRepos.Add(storageRepro);
                }

                res = new Storage<T>(storageRepos.ToArray<IDataRepository<T>>());

                if (!Warehouse.ContainsKey(typeof(T).Name))
                    Warehouse.Add(typeof(T).Name, res);
            }

            return res;
        }
    }
}
