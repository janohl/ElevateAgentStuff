using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IRequestPreCacheProcess<T>
    {
        Func<T, T> PreCacheProcess { get; set; }
    }

    public class Request<TP, T> : Request<TP>, IRequestPreCacheProcess<T>
    {
        public Request()
        {
        }

        public Request(TP parameters)
            : base(parameters)
        {
        }

        public Func<T, T> PreCacheProcess { get; set; }
    }

    public class Request<TP> : Request
    {
        public new TP Parameters
        {
            get { return (TP)base.Parameters; }
            set { base.Parameters = value; }
        }

        public Request()
        { }

        public Request(TP parameters)
            : base(parameters)
        {
        }
    }

    /// <summary>
    ///Request objet for holding infomration in order to retrieve an object from storege 
    /// </summary>
    public class Request
    {
        public object Parameters { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// Force get data from origin repro
        /// </summary>
        public bool ForceOriginRequest { get; set; }

        //Used for locking objects in editmode
        public bool LockForEdit { get; set; }
        public User LockBy { get; set; }

        public Request(object parameters)
        {
            this.Parameters = parameters;
        }

        public Request()
        {
            // TODO: Complete member initialization
        }
    }

}
