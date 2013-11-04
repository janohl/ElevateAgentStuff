using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class PathUtilities : IPathUtilities
    {
        public string ToAbsolute(string virtualPath)
        {
            return System.Web.VirtualPathUtility.ToAbsolute(virtualPath);
        }
    }
}