using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive; 
using System;

namespace MvLib.Reactive
{
    public class ReactiveCommand : IDisposable
    {
        private readonly Subject<Unit> executeSubject = new Subject<Unit>();
        private readonly Subject<Unit> onCompleteSubject = new Subject<Unit>(); 
        private IDisposable canExecuteSubscription;

        private readonly IObservable<bool> canExecuteObservable; 
        private bool canExecute = true;

        // Событие, уведомляющее о возможности выполнения команды
        public IObservable<Unit> Executed => executeSubject.AsObservable(); 
        public IObservable<Unit> OnComplete => onCompleteSubject.AsObservable();
        public IObservable<bool> CanExecuteChanged; 
         
        public ReactiveCommand() : this(Observable.Return(true)) {  } 
        public ReactiveCommand(IObservable<bool> canExecuteObservable)
        {
            this.canExecuteObservable = canExecuteObservable ?? throw new ArgumentNullException(nameof(canExecuteObservable));
            CanExecuteChanged = canExecuteObservable.DistinctUntilChanged();
            SubscribeToCanExecute();
        }
         
        public void Execute()
        {
            if (canExecute)
            {
                executeSubject.OnNext(Unit.Default);
            }
        }
         
        public bool CanExecute() => canExecute;  
        private void SubscribeToCanExecute()
        {
            canExecuteSubscription = canExecuteObservable
                .Subscribe(canExec =>
                {
                    canExecute = canExec;
                });
        } 
        public void Dispose()
        {
            executeSubject?.OnCompleted();
            canExecuteSubscription?.Dispose();
        }
    }
}
