using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Config
{
    static public class Global
    {
        public static T GetConfig<T>(string sectionName)
        {
            var c = (T)ConfigurationManager.GetSection(sectionName);
            return (T)c;
        }
    }
}
