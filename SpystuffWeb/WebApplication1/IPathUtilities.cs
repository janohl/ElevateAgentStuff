using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1
{
    public interface IPathUtilities
    {
        string ToAbsolute(string virtualPath);
    }
}
