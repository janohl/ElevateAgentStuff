using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Repositories.Config
{
    public class DBConfig : ConfigurationSection
    {
        [ConfigurationProperty("ConnectionsStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ((ConnectionStringSettingsCollection)(base["ConnectionsStrings"])); }
        }
    }
}
