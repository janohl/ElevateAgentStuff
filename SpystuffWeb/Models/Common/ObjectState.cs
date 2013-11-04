using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Common
{
    public enum ObjectState
    {
        Unchanged = 0,
        New = 1,
        Updated = 2,
        Deleted = 3
    }
}
