using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Config
{
    public interface IMemcachedConfig
    {
        string Servers { get; set; }
        string Bucket { get; set; }
    }
}
