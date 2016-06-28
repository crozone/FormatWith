using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormatWith.Internal
{
    public class DictionaryStringToObjectWrapper<T, U> : IDictionary<T, object> {
            private readonly IDictionary<T, U> inner;
            public DictionaryStringToObjectWrapper(IDictionary<T, U> wrapped) {
                this.inner = wrapped;
            }

            public void Add(T key, object value) { inner.Add(key, (U)value); }
            public bool ContainsKey(T key) { return inner.ContainsKey(key); }
            public ICollection<T> Keys { get { return inner.Keys; } }
            public bool Remove(T key) { return inner.Remove(key); }

            public bool TryGetValue(T key, out object value) {
                U temp;
                bool result = inner.TryGetValue(key, out temp);
                value = temp;
                return result;
            }

            public ICollection<object> Values { get { return inner.Values.Select(x => (object)x).ToArray(); } }

            public object this[T key]
            {
                get { return inner[key]; }
                set { inner[key] = (U)value; }
            }

            public void Add(KeyValuePair<T, object> item) { inner.Add(item.Key, (U)item.Value); }
            public void Clear() { inner.Clear(); }
            public bool Contains(KeyValuePair<T, object> item) { return inner.Contains(new KeyValuePair<T, U>(item.Key, (U)item.Value)); }
            public void CopyTo(KeyValuePair<T, object>[] array, int arrayIndex) { throw new NotImplementedException(); }
            public int Count { get { return inner.Count; } }
            public bool IsReadOnly { get { return false; } }
            public bool Remove(KeyValuePair<T, object> item) { return inner.Remove(item.Key); }

            public IEnumerator<KeyValuePair<T, object>> GetEnumerator() {
                foreach (var item in inner) {
                    yield return new KeyValuePair<T, object>(item.Key, item.Value);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                foreach (var item in inner) {
                    yield return new KeyValuePair<T, object>(item.Key, item.Value);
                }
            }
        }
}
