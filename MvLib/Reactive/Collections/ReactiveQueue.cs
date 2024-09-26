using System.Collections.Generic; 
using System.Reactive.Subjects;
using UnityEngine; 
using System; 

namespace MvLib.Reactive
{
    /// <summary>
    /// Представляет реактивную очередь, которая уведомляет наблюдателей о добавлении и удалении элементов.
    /// </summary>
    /// <typeparam name="T">Тип элементов в очереди.</typeparam>
    [Serializable] 
    public class ReactiveQueue<T> : IObservable<T>
    {
        private Queue<T> queue = new Queue<T>();
        private Subject<T> enqueueSubject = new Subject<T>();
        private Subject<T> dequeueSubject = new Subject<T>();

        /// <summary>
        /// Получает количество элементов в очереди.
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// Добавляет элемент в очередь и уведомляет наблюдателей об этом.
        /// </summary>
        /// <param name="item">Добавляемый элемент.</param>
        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            enqueueSubject.OnNext(item);
        }

        /// <summary>
        /// Удаляет элемент из очереди и уведомляет наблюдателей об этом.
        /// </summary>
        /// <returns>Удалённый элемент.</returns>
        /// <exception cref="InvalidOperationException">Если очередь пуста.</exception>
        public T Dequeue()
        {
            if (queue.Count > 0)
            {
                T item = queue.Dequeue();
                dequeueSubject.OnNext(item);
                return item;
            }
            throw new InvalidOperationException("Queue is empty");
        }

        /// <summary>
        /// Очищает очередь и уведомляет наблюдателей об этом.
        /// </summary>
        public void Clear()
        {
            queue.Clear(); 
            enqueueSubject.OnCompleted();
            dequeueSubject.OnCompleted();
        }

        /// <summary>
        /// Подписывается на уведомления об добавлении элементов в очередь.
        /// </summary>
        /// <param name="observer">Наблюдатель, который будет получать уведомления.</param>
        /// <returns>Объект для управления подпиской.</returns>
        public IDisposable Subscribe(IObserver<T> observer) => enqueueSubject.Subscribe(observer);

        /// <summary>
        /// Получает поток наблюдений для уведомлений об удалении элементов из очереди.
        /// </summary>
        public IObservable<T> OnDequeue => dequeueSubject;
    }
}
