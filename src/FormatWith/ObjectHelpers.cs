using System.Collections.Generic;
using System.Reflection;

namespace FormatWith {
    public static class ObjectHelpers
    {
        /// <summary>
        /// Creates a Dictionary from an objects properties, with the Key being the property's
        /// name and the Value being the properties value (of type object)
        /// </summary>
        /// <param name="properties">An object who's properties will be used</param>
        /// <returns>A <see cref="Dictionary"/> of property values</returns>
        public static Dictionary<string, object> GetPropertiesDictionary(object properties) {
            Dictionary<string, object> values = null;
            if (properties != null) {
                values = new Dictionary<string, object>();
                PropertyInfo[] propertyInfo = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in propertyInfo) {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }
            return values;
        }


    }
}
