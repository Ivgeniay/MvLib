using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace MvLib.Collections
{
    /// <summary>
    /// Реализация интерфейса `IDictionary` с поддержкой сериализации в Unity. 
    /// Хранит пары ключ-значение в виде списков для обеспечения совместимости с системой сериализации Unity.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значения в словаре.</typeparam> 
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public TValue this[TKey key]
        {
            get
            {
                int index = keys.IndexOf(key);
                if (index < 0)
                    throw new KeyNotFoundException();
                return values[index];
            }
            set
            {
                int index = keys.IndexOf(key);
                if (index < 0)
                    throw new KeyNotFoundException();
                values[index] = value;
            }
        }

        public ICollection<TKey> Keys => keys.AsReadOnly(); 
        public ICollection<TValue> Values => values.AsReadOnly(); 
        public int Count => keys.Count; 
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (keys.Contains(key))
                throw new ArgumentException("An element with the same key already exists.");
            keys.Add(key);
            values.Add(value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = keys.IndexOf(item.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The array is too small.");

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool Remove(TKey key)
        {
            int index = keys.IndexOf(key);
            if (index < 0)
                return false;
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = keys.IndexOf(item.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(values[index], item.Value))
                return false;
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // ISerializationCallbackReceiver Implementation 
        public void OnBeforeSerialize()
        {
            // Implement any necessary logic before serialization if needed
        }

        public void OnAfterDeserialize()
        {
            // Rebuild the dictionary after deserialization
            // This could be used to ensure consistency between keys and values
        }
    }
}
