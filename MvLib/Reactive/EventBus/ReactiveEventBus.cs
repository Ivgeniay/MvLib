using System.Collections.Generic; 
using System.Linq;
using System; 

namespace MvLib.Reactive
{
    /// <summary>
    /// Управляет событиями, позволяя регистрировать, получать и вызывать события.
    /// Хранит события в словаре с типами событий в качестве ключей.
    /// </summary>
    public class ReactiveEventBus
    {
        private readonly Dictionary<Type, List<object>> eventMap = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Регистрирует новое событие с заданным приоритетом.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <param name="priority">Приоритет события.</param>
        /// <returns>Созданное событие.</returns>
        public ReactiveEvent<T> RegisterEvent<T>(int priority = int.MaxValue)
        {
            var reactiveEvent = new ReactiveEvent<T> { Priority = priority };

            if (!eventMap.ContainsKey(typeof(T)))
            {
                eventMap[typeof(T)] = new List<object>();
            }
            eventMap[typeof(T)].Add(reactiveEvent);
            return reactiveEvent;
        }

        /// <summary>
        /// Получает все события заданного типа, отсортированные по приоритету.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <returns>Перечисление событий заданного типа, отсортированных по приоритету.</returns>
        public IEnumerable<ReactiveEvent<T>> GetEvents<T>()
        {
            if (eventMap.ContainsKey(typeof(T)))
            {
                return eventMap[typeof(T)].Cast<ReactiveEvent<T>>().OrderBy(e => e.Priority);
            }
            return Enumerable.Empty<ReactiveEvent<T>>();
        }

        /// <summary>
        /// Удаляет указанное событие.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <param name="reactiveEvent">Событие для удаления.</param>
        public void UnregisterEvent<T>(ReactiveEvent<T> reactiveEvent)
        {
            if (eventMap.ContainsKey(typeof(T)))
            {
                eventMap[typeof(T)].Remove(reactiveEvent);
                if (!eventMap[typeof(T)].Any())
                {
                    eventMap.Remove(typeof(T));
                }
            }
        }

        /// <summary>
        /// Вызывает все события заданного типа и передаёт данные события.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <param name="data">Данные события, которые будут переданы подписчикам.</param>
        public void RaiseAll<T>(T data)
        {
            foreach (var reactiveEvent in GetEvents<T>())
            {
                reactiveEvent.Raise(data);
            }
        }

        /// <summary>
        /// Регистрирует новое именованное событие с заданным приоритетом.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <param name="eventName">Имя события.</param>
        /// <param name="priority">Приоритет события.</param>
        /// <returns>Созданное именованное событие.</returns>
        public ReactiveEvent<T> RegisterNamedEvent<T>(string eventName, int priority = int.MaxValue)
        {
            var reactiveEvent = new ReactiveNamedEvent<T>(eventName) { Priority = priority };

            if (!eventMap.ContainsKey(typeof(T)))
            {
                eventMap[typeof(T)] = new List<object>();
            }

            eventMap[typeof(T)].Add(reactiveEvent);
            return reactiveEvent;
        }

        /// <summary>
        /// Получает именованное событие по имени.
        /// </summary>
        /// <typeparam name="T">Тип события.</typeparam>
        /// <param name="eventName">Имя события.</param>
        /// <returns>Именованное событие с указанным именем или null, если не найдено.</returns>
        public ReactiveEvent<T> GetNamedEvent<T>(string eventName)
        {
            if (eventMap.ContainsKey(typeof(T)))
            {
                var namedEvents = eventMap[typeof(T)].Cast<ReactiveNamedEvent<T>>();
                return namedEvents.FirstOrDefault(e => e.Name == eventName);
            }

            return null;
        }

        /// <summary>
        /// Очищает все зарегистрированные события.
        /// </summary>
        public void ClearAllEvents()
        {
            eventMap.Clear();
        }
    }


}
