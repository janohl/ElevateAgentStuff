using Raven.Client.Document;
using Repositories.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.RavenDB
{
    public class RavenDatabaseRepository<T> : BaseRepository<RavenDBConfig>, IDataRepository<T> where T : new()
    {
        DocumentStore _ds; 
        public RavenDatabaseRepository()
        {
            _ds = new DocumentStore();            
        }

        public T Get(Models.Request request)
        {
            using(var session = _ds.OpenSession())
            {
                var result = session.Query<T>().ToList<T>().FirstOrDefault<T>();
                
                return result;
            }
        }

        public void Save(T obj, Models.Request request = null)
        {
            using (var session = _ds.OpenSession())
            {
                session.Store(obj);
                session.SaveChanges();  
            }            
        }

        public int Delete(Models.Request request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetList(Models.Request request)
        {
            using (var session = _ds.OpenSession())
            {
                return session.Query<T>().ToList<T>();                
            }
        }

        public void SaveList(IEnumerable<T> list, Models.Request request = null)
        {
            using (var session = _ds.OpenSession())
            {
                foreach(var item in list)
                    session.Store(item);
                session.SaveChanges(); 
            }            
        }
    }
}
