using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Models.Common;

namespace Common
{
    public static class PropertyInfoHelper
    {
        /// <summary>
        /// Propertycache
        /// </summary>
        static Dictionary<Type, PropertyList> propCache = new Dictionary<Type, PropertyList>();

        internal static PropertyList GetProperties(object obj)
        {
            return GetProperties(obj.GetType());
        }

        internal static PropertyList GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }

        internal static PropertyList GetProperties(Type t)
        {
            PropertyList res;

            if (!propCache.TryGetValue(t, out res))
            {
                lock (propCache)
                {
                    if (!propCache.ContainsKey(t))
                    {
                        res = new PropertyList(t.GetProperties());
                        propCache.Add(t, res);
                    }
                    else
                        propCache.TryGetValue(t, out res);
                }
            }

            return res;
        }
    }

    public class PropertyList : List<PropertyInfo>
    {
        public PropertyList(PropertyInfo[] properties)
        {
            AddRange(properties);

            KeyProperty = properties.FirstOrDefault(p => p.IsDefined(typeof(KeyAttribute), true));

            DataListProperties = properties.Where(p => p.IsDefined(typeof(DataListAttribute), true));

            HasListProperties = DataListProperties.Any();

            DataProperties = properties.Where(p => p.IsDefined(typeof(DataAttribute), true));

            DataObjectProperties = properties.Where(p => p.IsDefined(typeof(DataObjectAttribute), true));
        }

        public PropertyInfo KeyProperty { get; private set; }

        public IEnumerable<PropertyInfo> DataProperties { get; private set; }

        public IEnumerable<PropertyInfo> DataObjectProperties { get; private set; }

        public bool HasListProperties { get; private set; }

        public IEnumerable<PropertyInfo> DataListProperties { get; private set; }
    }
}
