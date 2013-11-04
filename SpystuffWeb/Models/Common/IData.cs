using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Common
{
    public interface IData
    {
        int Id { get; set; }
        ObjectState State { get; set; }
        bool IsNew();
        bool IsDeleted();
        void Delete();
    }
}
