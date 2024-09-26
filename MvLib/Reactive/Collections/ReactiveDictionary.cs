using System.Collections.Generic;
using UnityEngine;
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Представляет реактивный словарь, который уведомляет наблюдателей о добавлении, обновлении, удалении элементов и очистке словаря.
    /// </summary>
    /// <typeparam name="TKey">Тип ключей в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значений в словаре.</typeparam>
    [Serializable]
    public class ReactiveDictionary<TKey, TValue> : IReactiveColection<TKey, TValue>
    {
        [SerializeField] private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public event Action<TKey, TValue> OnItemAdded;
        public event Action<TKey, TValue> OnItemUpdated;
        public event Action<TKey, TValue> OnItemRemoved;
        public event Action OnCleared;

        /// <summary>
        /// Получает количество элементов в словаре.
        /// </summary>
        public int Count => dictionary.Count;

        /// <summary>
        /// Добавляет элемент в словарь и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="key">Ключ элемента.</param>
        /// <param name="value">Значение элемента.</param>
        public void Add(TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                OnItemAdded?.Invoke(key, value);
            }
        }

        /// <summary>
        /// Удаляет элемент по ключу из словаря и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="key">Ключ удаляемого элемента.</param>
        /// <returns>true, если элемент был удалён, иначе false.</returns>
        public bool Remove(TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                dictionary.Remove(key);
                OnItemRemoved?.Invoke(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Очищает словарь и уведомляет наблюдателей об этом.
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            OnCleared?.Invoke();
        }

        /// <summary>
        /// Получает или устанавливает значение по ключу и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="key">Ключ элемента.</param>
        /// <returns>Значение по указанному ключу.</returns>
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                OnItemUpdated?.Invoke(key, value);
            }
        }

        /// <summary>
        /// Преобразует словарь в обычный <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>Копия словаря.</returns>
        public Dictionary<TKey, TValue> ToDictionary() => new Dictionary<TKey, TValue>(dictionary);

        /// <summary>
        /// Подписывается на уведомления об изменениях в словаре.
        /// </summary>
        /// <param name="observer">Наблюдатель, который будет получать уведомления.</param>
        /// <returns>Объект для управления подпиской.</returns>
        public IDisposable Subscribe(IObserver<(TKey, TValue)> observer)
        {
            OnItemAdded += (key, value) => observer.OnNext((key, value));
            return new Unsubscriber(() => OnItemAdded -= (key, value) => observer.OnNext((key, value)));
        }

        /// <summary>
        /// Освобождает все ресурсы, используемые объектом.
        /// </summary>
        public void Dispose()
        {
            OnItemAdded = null;
            OnItemUpdated = null;
            OnItemRemoved = null;
            OnCleared = null;
        }
    }
}
