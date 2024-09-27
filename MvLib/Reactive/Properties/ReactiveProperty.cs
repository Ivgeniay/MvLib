using System.Collections.Generic;
using System.Reactive.Linq;
using UnityEngine;  
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Реализует реактивное свойство с поддержкой наблюдателей.
    /// Используется для хранения значения и уведомления подписчиков об изменениях этого значения.
    /// </summary>
    /// <typeparam name="T">Тип значения свойства.</typeparam>
    [Serializable]
    public class ReactiveProperty<T> : IReactiveProperty<T>, IObservable<T>
    {
        [SerializeField] private T value;

        /// <summary>
        /// Событие, которое вызывается при изменении значения свойства.
        /// </summary> 
        public event Action<T> OnValueChanged;

        /// <summary>
        /// Значение свойства.
        /// При изменении значения уведомляет подписчиков.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    this.value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        public ReactiveProperty() : this(default) { }

        public ReactiveProperty(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Устанавливает новое значение и уведомляет подписчиков.
        /// </summary>
        /// <param name="newValue">Новое значение.</param>
        public void SetValueAndNotify(T newValue)
        {
            this.value = newValue;
            OnValueChanged?.Invoke(newValue);
        }

        /// <summary>
        /// Устанавливает новое значение без уведомления подписчиков.
        /// </summary>
        /// <param name="newValue">Новое значение.</param>
        /// <returns>Текущее свойство.</returns>
        public IReactiveProperty<T> SetValueWithoutNotify(T newValue)
        {
            this.value = newValue;
            return this;
        }

        /// <summary>
        /// Возвращает наблюдаемый объект для подписки на изменения значения.
        /// </summary>
        /// <returns>Наблюдаемый объект.</returns>
        public IObservable<T> AsObservable()
        {
            return Observable.FromEvent<T>(
                handler => OnValueChanged += handler,
                handler => OnValueChanged -= handler
            );
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(value);
            OnValueChanged += observer.OnNext;

            return new Unsubscriber(() => OnValueChanged -= observer.OnNext);
        }

        public IDisposable SubscribeWithotNotification(IObserver<T> observer)
        { 
            OnValueChanged += observer.OnNext; 
            return new Unsubscriber(() => OnValueChanged -= observer.OnNext);
        }

        public static implicit operator T(ReactiveProperty<T> property) => property.Value;
        public static implicit operator ReactiveProperty<T>(T value) => new ReactiveProperty<T>(value);

        public override string ToString() => Value.ToString();

        public void Dispose()
        {
            OnValueChanged = null;
        }
    }
}
