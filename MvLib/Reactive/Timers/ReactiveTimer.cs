using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive;
using System;

namespace MvLib.Reactive
{
    /// <summary>
    /// Реализует таймер с возможностью повторения и уведомления о тиках и завершении.
    /// Используется для выполнения действий через определённые интервалы времени.
    /// </summary>
    public class ReactiveTimer : IDisposable
    {
        private readonly float interval;
        private readonly float duration;
        private float elapsedTime;
        private bool isRunning;
        private bool isRepeating;

        private readonly Subject<float> tickSubject = new Subject<float>();
        private readonly Subject<Unit> completionSubject = new Subject<Unit>();

        /// <summary>
        /// Событие, которое вызывается при каждом тике таймера.
        /// </summary>
        public IObservable<float> OnTick => tickSubject.AsObservable();
        /// <summary>
        /// Событие, которое вызывается при завершении таймера.
        /// </summary>
        public IObservable<Unit> OnCompleted => completionSubject.AsObservable();

        public ReactiveTimer(float interval, bool isRepeating = false, float duration = -1f)
        {
            this.interval = interval;
            this.isRepeating = isRepeating;
            this.duration = duration;
            elapsedTime = 0f;
        }

        /// <summary>
        /// Запускает таймер.
        /// </summary>
        public void Start()
        {
            isRunning = true;
            elapsedTime = 0f;
        }

        /// <summary>
        /// Останавливает таймер.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Обновляет таймер. Должно вызываться в методе <see cref="Update"/> Unity.
        /// </summary>
        /// <param name="deltaTime">Время, прошедшее с последнего обновления.</param>
        public void Update(float deltaTime)
        {
            if (!isRunning) return;

            elapsedTime += deltaTime;

            if (duration > 0 && elapsedTime >= duration)
            {
                Stop();
                completionSubject.OnNext(Unit.Default);
                completionSubject.OnCompleted();
                Dispose();
                return;
            }

            // Если интервал прошел
            if (elapsedTime >= interval)
            {
                tickSubject.OnNext(elapsedTime);
                if (isRepeating)
                {
                    elapsedTime = 0f;
                }
                else
                {
                    Stop();
                    completionSubject.OnNext(Unit.Default);
                    completionSubject.OnCompleted();
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Сбрасывает таймер и перезапускает его.
        /// </summary>
        public void Reset()
        {
            elapsedTime = 0f;
            Start();
        }

        public void Dispose()
        {
            tickSubject?.OnCompleted();
            completionSubject?.OnCompleted();
            tickSubject?.Dispose();
            completionSubject?.Dispose();
        }
    }
}
