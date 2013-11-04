using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
    public class ObjectLockedException : Exception
    {
        public string LockedBy { get; set; }

        public ObjectLockedException(string message, string lockedBy) : base(message)
        {
            LockedBy = lockedBy;
        }
    }
}
