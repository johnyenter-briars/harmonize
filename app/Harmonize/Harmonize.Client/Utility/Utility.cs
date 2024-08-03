using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Client.Utility
{
    internal static class Utility
    {
        public static object? GetPropertyValue(object obj, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(propertyName);

            Type type = obj.GetType();

            PropertyInfo? propertyInfo = type.GetProperty(propertyName) 
                ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{type.FullName}'");

            return propertyInfo.GetValue(obj);
        }
    }
}
