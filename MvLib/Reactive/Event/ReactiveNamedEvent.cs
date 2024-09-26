using UnityEngine;
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Расширяет <see cref="ReactiveEvent{T}"/> добавлением имени события.
    /// Используется для событий с уникальными именами, которые могут быть идентифицированы по имени.
    /// </summary>
    /// <typeparam name="T">Тип данных события.</typeparam>
    [Serializable]
    public sealed class ReactiveNamedEvent<T> : ReactiveEvent<T>
    {
        /// <summary>
        /// Имя события, используемое для идентификации события.
        /// </summary>
        [SerializeField] public string Name { get; }

        /// <summary>
        /// Конструктор с именем события.
        /// </summary>
        /// <param name="name">Имя события.</param>
        public ReactiveNamedEvent(string name)
        {
            Name = name;
        }
    }
}
