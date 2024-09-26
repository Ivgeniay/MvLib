using System.Collections.Generic; 
using UnityEngine;
using System; 

namespace MvLib.Reactive
{
    /// <summary>
    /// Представляет реактивный список, который уведомляет наблюдателей об изменениях в списке, таких как добавление, удаление и обновление элементов.
    /// </summary>
    /// <typeparam name="T">Тип элементов в списке.</typeparam>
    [Serializable]
    public class ReactiveList<T> : IReactiveColection<T>
    {
        [SerializeField] private List<T> list = new List<T>();

        public event Action<List<T>> OnChange;
        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnListCleared;
        public event Action<int, T> OnItemUpdated;

        /// <summary>
        /// Получает количество элементов в списке.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Добавляет элемент в список и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="item">Добавляемый элемент.</param>
        public void Add(T item)
        {
            list.Add(item);
            OnItemAdded?.Invoke(item);
            OnChange?.Invoke(new List<T>(list)); // Создаем копию списка для уведомления
        }

        /// <summary>
        /// Удаляет элемент из списка и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="item">Удаляемый элемент.</param>
        /// <returns>true, если элемент был удалён, иначе false.</returns>
        public bool Remove(T item)
        {
            if (list.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
                OnChange?.Invoke(new List<T>(list));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удаляет элемент по индексу из списка и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента.</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < list.Count)
            {
                T item = list[index];
                list.RemoveAt(index);
                OnItemRemoved?.Invoke(item);
                OnChange?.Invoke(new List<T>(list));
            }
        }

        /// <summary>
        /// Очищает список и уведомляет наблюдателей об этом.
        /// </summary>
        public void Clear()
        {
            foreach (var item in list) OnItemRemoved?.Invoke(item);

            list.Clear();
            OnListCleared?.Invoke();
            OnChange?.Invoke(new List<T>(list));
        }

        /// <summary>
        /// Обновляет элемент по индексу и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="index">Индекс элемента для обновления.</param>
        /// <param name="item">Новый элемент.</param>
        public void UpdateAt(int index, T item)
        {
            if (index >= 0 && index < list.Count)
            {
                list[index] = item;
                OnItemUpdated?.Invoke(index, item);
                OnChange?.Invoke(new List<T>(list));
            }
        }

        /// <summary>
        /// Получает или устанавливает элемент по индексу и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        /// <returns>Элемент по указанному индексу.</returns>
        public T this[int index]
        {
            get => list[index];
            set
            {
                if (index >= 0 && index < list.Count)
                {
                    list[index] = value;
                    OnItemUpdated?.Invoke(index, value);
                    OnChange?.Invoke(new List<T>(list));
                }
            }
        }

        /// <summary>
        /// Преобразует список в обычный список <see cref="List{T}"/>.
        /// </summary>
        /// <returns>Копия списка.</returns>
        public List<T> ToList() => new List<T>(list);

        /// <summary>
        /// Подписывается на уведомления об изменениях в списке.
        /// </summary>
        /// <param name="observer">Наблюдатель, который будет получать уведомления.</param>
        /// <returns>Объект для управления подпиской.</returns>
        public IDisposable Subscribe(IObserver<List<T>> observer)
        { 
            observer.OnNext(new List<T>(list)); 
            OnChange += observer.OnNext;

            return new Unsubscriber(() => OnChange -= observer.OnNext);
        }

        /// <summary>
        /// Подписывается на уведомления о добавлении и удалении элементов.
        /// </summary>
        /// <param name="observer">Наблюдатель, который будет получать уведомления.</param>
        /// <returns>Объект для управления подпиской.</returns>
        public IDisposable SubscribeToAdditions(IObserver<T> observer)
        {
            OnItemAdded += observer.OnNext;
            OnItemRemoved += observer.OnNext;

            return new Unsubscriber(() =>
            {
                OnItemAdded -= observer.OnNext;
                OnItemRemoved -= observer.OnNext;
            });
        }

        /// <summary>
        /// Освобождает все ресурсы, используемые объектом.
        /// </summary>
        public void Dispose()
        {
            OnChange = null;
            OnItemAdded = null;
            OnItemRemoved = null;
            OnListCleared = null;
            OnItemUpdated = null; 
        }

        public IDisposable Subscribe(IObserver<T> observer) => SubscribeToAdditions(observer); 

        /// <summary>
        /// Преобразует реактивный список в обычный <see cref="List{T}"/>.
        /// </summary>
        /// <param name="reactiveList">Реактивный список для преобразования.</param>
        /// <returns>Список элементов.</returns>
        public static implicit operator List<T>(ReactiveList<T> reactiveList) => reactiveList.list;

    }
}
