using System.Collections.Generic;
using System;

namespace MvLib.Reactive
{
    public interface IReactiveColection<T> : IReactive<T>
    {
        public int Count { get; }
        public void Add(T item);
        public bool Remove(T item); 
        public void Clear(); 
    }

    public interface IReactiveColection<TKey, TValue> : IReactive<(TKey, TValue)>
    {
        public int Count { get; }
        public void Add(TKey key, TValue value);
        public bool Remove(TKey key);
        public void Clear();
    }
}
