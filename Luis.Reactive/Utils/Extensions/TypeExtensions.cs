using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive.Utils.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> PropertiesOfType<IType>(this Type type)
        {
            return from property in type.GetProperties()
                   where typeof(IType).IsAssignableFrom(property.PropertyType)
                   select property;
        }
    }
}
