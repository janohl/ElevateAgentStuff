namespace Repositories
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Models;

    /// <summary>
    /// Factoryclass for getting the correct database repro based on type.
    /// Naming convension for Repros are <name>DatabaseRepository were name is name of type
    /// Example AliasDatabaseRepository
    /// </summary>
    public static class DatabaseRepositoryFactory
    {
        /// <summary>
        /// Always return default DatabaseRepository()
        /// </summary>
        /// <returns></returns>
        public static IDataRepository<object> GetRep()
        {
            return DatabaseRepositoryFactory.GetRep<object>();
        }


        /// <summary>
        /// Returns specific repository based on type if it exists. Namingconvension is
        /// [Typename]DatabaseRepository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IDataRepository database repository</returns>
        public static IDataRepository<T> GetRep<T>() where T : new()
        {
            IDataRepository<T> rep = null;

            //if(warehouse.ContainsKey(typeof(T)))  
            //	rep = warehouse[typeof(T)];

            //if (rep == null)
            //{

            var type = typeof(T);
            var identifier = String.Format("{0}.{1}", type.Namespace.Split('.').Last(), type.Name);
            switch (identifier)
            {
                case "Products.Product":
                    rep = new Products.ProductDatabaseRepository<T>();
                    break;
                default:
                    rep = new DatabaseRepository<T>();
                    break;
            }

            return rep;
        }
    }
}



/*
//Dummy
namespace Nelly.DAL.Repositories
{
	using Nelly.Models;
	using System.Collections.Generic;
	using System;

    public static class DatabaseRepositoryFactory
    {
		public static IDataRepository GetRep()
		{ 
			return null;
		}

        public static IDataRepository GetRep<T>()
        {
		return null;
		}
	}

}



*/


