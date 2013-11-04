using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDataRepository<T> where T : new()
    {
        /// <summary>
        /// Get data instance
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        T Get(Request request);

        /// <summary>
        /// Save instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="request"></param>
        void Save(T obj, Request request = null);

        /// <summary>
        /// Delete instance
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int Delete(Request request);

        /// <summary>
        /// Get list of instances
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(Request request);

        /// <summary>
        /// SAve list of instances
        /// </summary>
        /// <param name="list"></param>
        /// <param name="request"></param>
        void SaveList(IEnumerable<T> list, Request request = null);
    }
}
