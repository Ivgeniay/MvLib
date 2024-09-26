using UnityEngine;
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Реализует механизм событий с возможностью подписки и уведомления о событиях.
    /// Используется для обработки событий, где тип данных события определён обобщённым параметром <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Тип данных события, которое обрабатывается.</typeparam>
    [Serializable]
    public class ReactiveEvent<T>
    {
        /// <summary>
        /// Событие, которое вызывается при возникновении события.
        /// </summary>
        public event Action<T> OnEvent;

        /// <summary>
        /// Приоритет события, определяет порядок вызова при множественных событиях.
        /// </summary>
        [field: SerializeField] public int Priority { get; set; } = int.MaxValue;

        /// <summary>
        /// Возбуждает событие и передаёт данные события подписчикам.
        /// </summary>
        /// <param name="eventData">Данные события, которые будут переданы подписчикам.</param>
        public virtual void Raise(T eventData)
        {
            OnEvent?.Invoke(eventData);
        }

        /// <summary>
        /// Подписывается на событие и возвращает объект <see cref="IDisposable"/> для отписки.
        /// </summary>
        /// <param name="handler">Метод, который будет вызван при возникновении события.</param>
        /// <returns>Объект <see cref="IDisposable"/>, который можно использовать для отписки от события.</returns>
        public virtual IDisposable Subscribe(Action<T> handler)
        {
            OnEvent += handler;
            return new Unsubscriber(() => OnEvent -= handler);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly Action _unsubscribe;

            public Unsubscriber(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose()
            {
                _unsubscribe();
            }
        }
    }
}
