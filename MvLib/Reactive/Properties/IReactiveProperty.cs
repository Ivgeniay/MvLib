using System;

namespace MvLib.Reactive
{
    public interface IReactiveProperty<T> : IReactive<T>
    {
        public T Value { get; set; }
        public IReactiveProperty<T> SetValueWithoutNotify(T value);
    }
}
