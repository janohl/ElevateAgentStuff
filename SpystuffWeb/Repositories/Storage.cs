using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IStorage<T> where T : new()
    {
        T Get(Request request);
        //void Save<T>(T obj, Request request = null);
        //int Delete<T>(Request request);
        IEnumerable<T> GetList(Request request = null);
        //void SaveList<T>(IEnumerable<T> list, Request request = null);		
    }

    public class Storage<T> : IStorage<T> where T : new()
    {
        private readonly IDataRepository<T>[] _repositories;
        private int ReproCount { get; set; }

        public List<IDataRepository<T>> Repositories
        {
            get
            {
                return _repositories.ToList<IDataRepository<T>>();
            }
        }

        public Storage(IDataRepository<T>[] repositories)
        {
            _repositories = repositories;
            ReproCount = _repositories.Count();
        }

        public IEnumerable<T> GetList(Request request = null)
        {
            var offset = 0;
            if (request != null && request.ForceOriginRequest)
                offset = ReproCount - 1;

            for (int reproIndex = offset; reproIndex < ReproCount; reproIndex++)
            {
                var obj = _repositories[reproIndex].GetList(request);

                if (obj != null)
                {
                    while (reproIndex > 0)
                    {
                        _repositories[reproIndex - 1].SaveList(obj, request);
                        reproIndex--;
                    }

                    return obj;
                }
            }

            return null;
        }

        public T Get(Request request)
        {
            var offset = 0;
            if (request.ForceOriginRequest)
                offset = ReproCount - 1;

            for (int reproIndex = offset; reproIndex < ReproCount; reproIndex++)
            {
                var obj = _repositories[reproIndex].Get(request);

                if (obj != null)
                {
                    while (reproIndex > 0)
                    {
                        _repositories[reproIndex - 1].Save(obj, request);

                        reproIndex--;
                    }

                    return (T)obj;
                }
            }

            return default(T);
        }
    }
}
