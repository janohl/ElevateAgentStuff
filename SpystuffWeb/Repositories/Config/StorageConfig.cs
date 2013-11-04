using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Repositories.Config
{
    public class StorageConfig : ConfigurationSection
    {
        [ConfigurationProperty("types")]
        public ConfigurationCollection<ClassTypeItem> Types
        {
            get { return ((ConfigurationCollection<ClassTypeItem>)(base["types"])); }
        }
    }


    public class ClassTypeItem : ConfigurationElement
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

        public override string ToString()
        {
            return Name;
        }

        [ConfigurationProperty("repros")]
        public ConfigurationCollection<ReproItem> Repros
        {
            get { return ((ConfigurationCollection<ReproItem>)(base["repros"])); }
        }
    }

    public class ReproItem : ConfigurationElement
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

        public override string ToString()
        {
            return Name;
        }
    }

}
