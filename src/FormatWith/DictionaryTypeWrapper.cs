using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FormatWith {
    public class DictionaryTypeWrapper : IDictionary<string, object> {
        private object rootObject;
        private Type rootObjectType;
        private static BindingFlags propertyFlags = BindingFlags.Instance | BindingFlags.Public;
        private Dictionary<string, PropertyInfo> _allProperties = null;
        private Dictionary<string, PropertyInfo> allProperties
        {
            get
            {
                // lazily generate properties name dictionary
                if (_allProperties == null) {
                    _allProperties = rootObjectType.GetProperties(propertyFlags).ToDictionary(p => p.Name, p => p);
                }

                return _allProperties;
            }
        }


        public DictionaryTypeWrapper(object root) {
            if (root == null) throw new ArgumentNullException(nameof(root));
            this.rootObject = root;
            rootObjectType = root.GetType();
        }

        private bool TryGetParameter(string parameterName, out object parameterObject) {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            parameterObject = null;

            PropertyInfo property;
            // accessing allProperties triggers lazy initialisation of the dictionary
            if (!allProperties.TryGetValue(parameterName, out property)) {
                return false;
            }

            // get the object from the property
            parameterObject = property.GetValue(rootObject);

            return true;
        }

        public object this[string key]
        {
            get
            {
                object returnObject;
                if (!TryGetParameter(key, out returnObject)) {
                    throw new KeyNotFoundException($"A parameter with the name {key} was not found");
                }

                return returnObject;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public int Count
        {
            get
            {
                return allProperties.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return allProperties.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return allProperties.Select(kvp => {
                    object paramObject;
                    TryGetParameter(kvp.Key, out paramObject);
                    return paramObject;
                }).ToList();
            }
        }

        public void Add(KeyValuePair<string, object> item) {
            throw new NotSupportedException();
        }

        public void Add(string key, object value) {
            throw new NotSupportedException();
        }

        public void Clear() {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            object paramObject;
            return (TryGetParameter(item.Key, out paramObject) && paramObject == item.Value);
        }

        public bool ContainsKey(string key) {
            return allProperties.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            foreach (string key in this.Keys) {
                object paramObject;
                this.TryGetParameter(key, out paramObject);
                yield return new KeyValuePair<string, object>(key, paramObject);
            }

            yield break;
        }

        public bool Remove(KeyValuePair<string, object> item) {
            throw new NotSupportedException();
        }

        public bool Remove(string key) {
            throw new NotSupportedException();
        }

        public bool TryGetValue(string key, out object value) {
            return TryGetParameter(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }
    }
}
