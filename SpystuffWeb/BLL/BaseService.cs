using Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BaseService<T> where T: new()
    {
        protected log4net.ILog Logger;

        protected IStorage<T> storage;

        // Removed, inject storage instead.
        ////protected virtual void InitStorage()
        ////{
        ////    storage = StorageFactory<T>.GetStorage();  
        ////}

        public BaseService(IStorage<T> injectedStorage)
        {
            Logger = log4net.LogManager.GetLogger(this.GetType().Name);
            //InitStorage();
            storage = injectedStorage;
        }
    }
}
