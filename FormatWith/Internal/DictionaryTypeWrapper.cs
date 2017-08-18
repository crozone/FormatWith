using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FormatWith.Internal
{
    /// <summary>
    /// A dictionary that wraps any type to provide indexed access to the values of its properties.
    /// </summary>
    internal class DictionaryTypeWrapper : IDictionary<string, object>
    {
        private object rootObject;
        private Type rootObjectType;
        private static BindingFlags propertyFlags = BindingFlags.Instance | BindingFlags.Public;
        private Dictionary<string, PropertyInfo> _allProperties = null;
        private Dictionary<string, PropertyInfo> AllProperties {
            get {
                // lazily generate properties name dictionary
                if (_allProperties == null)
                {
                    _allProperties = rootObjectType.GetProperties(propertyFlags).ToDictionary(p => p.Name, p => p);
                }

                return _allProperties;
            }
        }


        public DictionaryTypeWrapper(object root)
        {
            rootObject = root ?? throw new ArgumentNullException(nameof(root));
            rootObjectType = root.GetType();
        }

        private bool TryGetParameter(string parameterName, out object parameterObject)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            parameterObject = null;

            // accessing allProperties triggers lazy initialisation of the dictionary
            if (!AllProperties.TryGetValue(parameterName, out PropertyInfo property))
            {
                return false;
            }

            // get the object from the property
            parameterObject = property.GetValue(rootObject);

            return true;
        }

        public object this[string key] {
            get {
                if (!TryGetParameter(key, out object returnObject))
                {
                    throw new KeyNotFoundException($"A parameter with the name {key} was not found");
                }

                return returnObject;
            }
            set {
                throw new NotSupportedException();
            }
        }

        public int Count {
            get {
                return AllProperties.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return true;
            }
        }

        public ICollection<string> Keys {
            get {
                return AllProperties.Keys;
            }
        }

        public ICollection<object> Values {
            get {
                return AllProperties.Select(kvp =>
                {
                    TryGetParameter(kvp.Key, out object paramObject);
                    return paramObject;
                }).ToList();
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        public void Add(string key, object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return (TryGetParameter(item.Key, out object paramObject) && paramObject == item.Value);
        }

        public bool ContainsKey(string key)
        {
            return AllProperties.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (string key in this.Keys)
            {
                TryGetParameter(key, out object paramObject);
                yield return new KeyValuePair<string, object>(key, paramObject);
            }

            yield break;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(string key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            return TryGetParameter(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}
