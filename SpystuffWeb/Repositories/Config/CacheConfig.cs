using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Repositories.Config;

namespace Repositories.Config
{
    public class CacheConfig : ConfigurationSection
    {
        [ConfigurationProperty("types")]
        public ConfigurationCollection<ConfigurationTypeItem> Types
        {
            get { return ((ConfigurationCollection<ConfigurationTypeItem>)(base["types"])); }
        }

        [ConfigurationProperty("defaultTTL", DefaultValue = "00:00:60", IsKey = false, IsRequired = true)]
        public string DefaultTTL
        {
            get
            {
                return ((string)(base["defaultTTL"]));
            }

            set
            {
                base["defaultTTL"] = value;
            }
        }
    }

    public class ConfigurationTypeItem : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return ((string)(base["name"]));
            }

            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("ttl", DefaultValue = "00:00:60", IsKey = false, IsRequired = true)]
        public string TTL
        {
            get
            {
                return ((string)(base["ttl"]));
            }

            set
            {
                base["ttl"] = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
