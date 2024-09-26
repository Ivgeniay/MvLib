using System;

namespace MvLib.Reactive
{
    public interface IReactive<T> : IDisposable, IObservable<T> { } 
    public interface IReactive<TKey, TValue> : IDisposable, IObservable<TKey> { }
}
