using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Repositories.Config
{
    [ConfigurationCollection(typeof(ConfigurationElement))]
    public class ConfigurationCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.ToString();
        }

        public T this[int idx]
        {
            get
            {
                return (T)BaseGet(idx);
            }
        }

        public new T this[string key]
        {
            get
            {
                return (T)BaseGet(key);
            }
        }
    }
}
