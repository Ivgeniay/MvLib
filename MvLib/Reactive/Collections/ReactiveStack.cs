using System.Collections.Generic; 
using UnityEngine;
using System;
using System.Reactive.Subjects;

namespace MvLib.Reactive
{
    /// <summary>
    /// Представляет стек, который поддерживает реактивное программирование, позволяя подписчикам получать уведомления о добавлении и удалении элементов.
    /// </summary>
    /// <typeparam name="T">Тип элементов, которые хранятся в стеке.</typeparam>
    [Serializable]
    public class ReactiveStack<T> : IObservable<T>
    {
        private Stack<T> stack = new Stack<T>();
        private Subject<T> pushSubject = new Subject<T>();
        private Subject<T> popSubject = new Subject<T>();

        /// <summary>
        /// Получает количество элементов в стеке.
        /// </summary>
        public int Count => stack.Count;

        /// <summary>
        /// Добавляет элемент на вершину стека и уведомляет подписчиков о добавлении.
        /// </summary>
        /// <param name="item">Элемент, который нужно добавить в стек.</param>
        public void Push(T item)
        {
            stack.Push(item);
            pushSubject.OnNext(item);
        }

        /// <summary>
        /// Удаляет элемент с вершины стека, если стек не пуст, и уведомляет подписчиков о удалении.
        /// </summary>
        /// <returns>Удалённый элемент.</returns>
        /// <exception cref="InvalidOperationException">Бросается, если стек пуст.</exception>
        public T Pop()
        {
            if (stack.Count > 0)
            {
                T item = stack.Pop();
                popSubject.OnNext(item);
                return item;
            }
            throw new InvalidOperationException("Stack is empty");
        }

        /// <summary>
        /// Очищает стек и завершает все активные уведомления о добавлении и удалении элементов.
        /// </summary>
        public void Clear()
        {
            stack.Clear();
            pushSubject.OnCompleted();
            popSubject.OnCompleted();
        }

        /// <summary>
        /// Подписывается на уведомления о добавлении элементов в стек.
        /// </summary>
        /// <param name="observer">Объект, реализующий интерфейс <see cref="IObserver{T}"/>, который будет получать уведомления.</param>
        /// <returns>Объект <see cref="IDisposable"/>, который можно использовать для отмены подписки.</returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return pushSubject.Subscribe(observer);
        }

        /// <summary>
        /// Получает наблюдаемый объект для уведомлений о удалении элементов из стека.
        /// </summary>
        public IObservable<T> OnPop => popSubject;
    }
}
