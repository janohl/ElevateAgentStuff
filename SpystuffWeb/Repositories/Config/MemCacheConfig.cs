using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Repositories.Config;

namespace Repositories.Config
{
    public class MemcacheConfig : CacheConfig, IMemcachedConfig
    {
        [ConfigurationProperty("servers", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Servers
        {
            get
            {
                return ((string)(base["servers"]));
            }

            set
            {
                base["servers"] = value;
            }
        }

        [ConfigurationProperty("bucket", DefaultValue = "default", IsKey = false, IsRequired = false)]
        public string Bucket
        {
            get
            {
                return ((string)(base["bucket"]));
            }

            set
            {
                base["bucket"] = value;
            }
        }
    }
}
