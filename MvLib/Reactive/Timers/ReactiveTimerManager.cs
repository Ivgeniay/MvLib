using System.Collections.Generic; 
using UnityEngine;
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Управляет списком таймеров в Unity.
    /// Позволяет добавлять, удалять и обновлять таймеры.
    /// </summary>
    public class ReactiveTimerManager : MonoBehaviour
    {
        private readonly List<ReactiveTimer> timers = new List<ReactiveTimer>();
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        /// <summary>
        /// Добавляет таймер и подписывается на его события.
        /// </summary>
        /// <param name="timer">Таймер для добавления.</param>
        /// <param name="onTick">Действие, которое будет выполняться при каждом тике таймера.</param>
        /// <param name="onComplete">Действие, которое будет выполняться при завершении таймера.</param>
        public void AddTimer(ReactiveTimer timer, Action<float> onTick = null, Action onComplete = null)
        {
            timers.Add(timer);
             
            if (onTick != null)
            {
                var tickSubscription = timer.OnTick.Subscribe(onTick);
                subscriptions.Add(tickSubscription);
            }

            if (onComplete != null)
            {
                var completionSubscription = timer.OnCompleted.Subscribe(_ => onComplete());
                subscriptions.Add(completionSubscription);
            }

            timer.Start();
        }

        /// <summary>
        /// Удаляет таймер из списка и освобождает его ресурсы.
        /// </summary>
        /// <param name="timer">Таймер для удаления.</param>
        public void RemoveTimer(ReactiveTimer timer)
        {
            if (timers.Contains(timer))
            {
                timers.Remove(timer);
                timer.Dispose();
            }
        }
         
        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                timers[i].Update(deltaTime);
            }
        }
         
        private void OnDestroy()
        {
            ClearTimers();
        }

        /// <summary>
        /// Очищает все таймеры и освобождает их ресурсы.
        /// </summary>
        public void ClearTimers()
        {
            foreach (var timer in timers)
            {
                timer.Dispose();
            }

            timers.Clear();

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            } 
            subscriptions.Clear();
        }
    }
}
