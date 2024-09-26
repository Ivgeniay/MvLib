using System; 

namespace MvLib.Reactive
{
    /// <summary>
    /// Реализует механизм отписки от списка наблюдателей.
    /// Используется для удаления наблюдателя из списка.
    /// </summary>
    internal class Unsubscriber : IDisposable
    {
        private readonly Action unsubscribeAction;

        public Unsubscriber(Action unsubscribeAction)
        {
            this.unsubscribeAction = unsubscribeAction;
        }

        public void Dispose()
        {
            unsubscribeAction();
        }
    }
}
