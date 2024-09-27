using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using MvLib.Reactive;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace MvLib
{
    /// <summary>
    /// Управляет связыванием между свойствами `ReactiveProperty` и элементами пользовательского интерфейса `BaseField`.
    /// Обеспечивает создание привязок, их обновление и удаление. Также реализует интерфейс `IDisposable` для освобождения ресурсов.
    /// </summary>
    /// <typeparam name="T">Тип данных, с которым работает этот класс.</typeparam>
    [Serializable]
    public class Binder<T> : IDisposable
    {
        [SerializeField] private List<BindModel<T>> bindModels = new List<BindModel<T>>();


        public PropertyField Bind(SerializedObject serializedObject, string reactiveProp, BaseField<T> field)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo reactiveField = t.GetFields().Where(e => e.Name == reactiveProp).FirstOrDefault();
            if (reactiveField == null)
            {
                Debug.LogError($"Field {reactiveProp} not found in {serializedObject.targetObject}");
                return null;
            }
            ReactiveProperty<T> reactivePropValue = reactiveField.GetValue(serializedObject.targetObject) as ReactiveProperty<T>;
            SerializedProperty sp = serializedObject.FindProperty(reactiveProp);
            PropertyField valueField = new PropertyField(sp);
            field.value = reactivePropValue.Value;
            valueField.RegisterCallback<ChangeEvent<T>>(evt => reactivePropValue.SetValueAndNotify(evt.newValue));
            Bind(reactivePropValue, field);

            return valueField;
        }

        public void Bind(ReactiveProperty<T> reactiveProp, BaseField<T> field)
        {
            EventCallback<ChangeEvent<T>> callback = evt => { reactiveProp.SetValueAndNotify(evt.newValue); }; 
            field.RegisterCallback<ChangeEvent<T>>(callback);

            IDisposable disposable = reactiveProp.AsObservable().Subscribe((value) =>
            {
                field.value = value;
            });

            BindModel<T> bindModel = bindModels.FirstOrDefault(e => e.ReactiveProperty == reactiveProp && e.field == field);
            if (bindModel == null)
            {
                bindModels.Add(
                    new BindModel<T>
                    {
                        ReactiveProperty = reactiveProp,
                        field = field,
                        Delegate = callback,
                        Disposable = disposable
                    }
                );
            }
            //else
            //{
                //UnBind(reactiveProp, field);
            //}

        }

        public void UnBind(ReactiveProperty<T> reactiveProp, BaseField<T> field)
        {
            BindModel<T> bindModel = bindModels.Find(e => e.ReactiveProperty == reactiveProp && e.field == field);
            if (bindModel != null)
            {
                if (bindModel.Delegate != null) field.UnregisterCallback(bindModel.Delegate);
                if (bindModel.Disposable != null) bindModel.Disposable.Dispose();
            }
            bindModels.Remove(bindModel);
        }

        public void UnBindAll(ReactiveProperty<T> reactiveProp)
        {
            List<BindModel<T>> bindModels = this.bindModels.FindAll(e => e.ReactiveProperty == reactiveProp);
            for (int i = 0; i < bindModels.Count; i++)
            {
                UnBind(reactiveProp, bindModels[i].field);
            }
        }

        public void Dispose()
        {
            if (bindModels == null) return;

            for (int i = 0; i < bindModels.Count; i++)
            {
                UnBind(bindModels[i].ReactiveProperty, bindModels[i].field);
            }
            bindModels.Clear();
        }
    }

    /// <summary>
    /// Управляет привязкой между свойствами `ReactiveProperty` и элементами пользовательского интерфейса `BaseField`.
    /// Поддерживает преобразование типов между `TSource` и `TTarget`, если они различны.
    /// Обеспечивает создание привязок, их обновление и удаление. Также реализует интерфейс `IDisposable` для освобождения ресурсов.
    /// </summary>
    /// <typeparam name="TSource">Тип данных, из которого будет происходить привязка.</typeparam>
    /// <typeparam name="TTarget">Тип данных, к которому будет происходить привязка (если требуется преобразование).</typeparam>
    [Serializable]
    public class Binder<TSource, TTarget> : IDisposable
    {
        [SerializeField] private readonly Func<TSource, TTarget> _converter;
        [SerializeField] private readonly List<BindModel<TSource, TTarget>> _bindModels = new();

        public Binder(Func<TSource, TTarget> converter)
        {
            _converter = converter;
        }

        /// <summary>
        /// Привязывает `ReactiveProperty` к полю `BaseField` с возможностью преобразования типов.
        /// </summary>
        /// <param name="reactiveProp">Реактивное свойство, которое будет отслеживаться.</param>
        /// <param name="field">UI поле, к которому будет привязано значение.</param>
        public void Bind(ReactiveProperty<TSource> reactiveProp, BaseField<TTarget> field)
        {
            EventCallback<ChangeEvent<TTarget>> callback = evt =>
            {
                // Если требуется обратное преобразование (от TTarget к TSource), его можно также добавить здесь
                // Например, через другой делегат _reverseConverter
            };

            field.RegisterCallback<ChangeEvent<TTarget>>(callback);
            IDisposable disposable = reactiveProp.AsObservable().Subscribe(value =>
            {
                field.value = _converter(value); // Преобразуем значение из TSource в TTarget
            });

            var bindModel = _bindModels.FirstOrDefault(e => e.ReactiveProperty == reactiveProp && e.Field == field);
            if (bindModel == null)
            {
                _bindModels.Add(
                    new BindModel<TSource, TTarget>
                    {
                        ReactiveProperty = reactiveProp,
                        Field = field,
                        Delegate = callback,
                        Disposable = disposable
                    }
                );
            }
            else
            {
                Unbind(reactiveProp, field);
            }
        }

        /// <summary>
        /// Отменяет привязку между реактивным свойством и полем.
        /// </summary>
        public void Unbind(ReactiveProperty<TSource> reactiveProp, BaseField<TTarget> field)
        {
            if (reactiveProp == null || field == null) return;

            var bindModel = _bindModels.Find(e => e.ReactiveProperty == reactiveProp && e.Field == field);
            if (bindModel != null)
            {
                if (bindModel.Delegate != null) field.UnregisterCallback(bindModel.Delegate);
                if (bindModel.Disposable != null) bindModel.Disposable.Dispose();
            }
            _bindModels.Remove(bindModel);
        }

        /// <summary>
        /// Отменяет привязку всех полей для указанного реактивного свойства.
        /// </summary>
        public void UnbindAll(ReactiveProperty<TSource> reactiveProp)
        {
            var models = _bindModels.FindAll(e => e.ReactiveProperty == reactiveProp);
            for (int i = 0; i < models.Count; i++)
            {
                Unbind(reactiveProp, models[i].Field);
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < _bindModels.Count; i++)
            {
                Unbind(_bindModels[i].ReactiveProperty, _bindModels[i].Field);
            }
            _bindModels.Clear();
        }
    }

    public class ReactivePropertyBinder<T> : IDisposable
    {
        private readonly ReactiveProperty<T> _resultProperty;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly Func<IEnumerable<T>, T> _aggregator;

        /// <summary>
        /// Создает новый биндер для привязки нескольких ReactiveProperty к одному с использованием агрегации.
        /// </summary>
        /// <param name="resultProperty">Реактивное свойство, в которое будет записываться результат.</param>
        /// <param name="aggregator">Функция агрегации значений, которая будет вычислять новое значение для resultProperty на основе значений привязанных свойств.</param>
        public ReactivePropertyBinder(ReactiveProperty<T> resultProperty, Func<IEnumerable<T>, T> aggregator)
        {
            _resultProperty = resultProperty;
            _aggregator = aggregator;
        }

        /// <summary>
        /// Привязывает несколько ReactiveProperty к одному.
        /// </summary>
        /// <param name="properties">Список привязываемых ReactiveProperty.</param>
        public void Bind(params ReactiveProperty<T>[] properties)
        {
            // Создаем подписку для каждого привязанного свойства
            foreach (var prop in properties)
            {
                IDisposable subscription = prop.AsObservable().Subscribe(_ =>
                {
                    // Агрегируем значения всех привязанных свойств
                    var aggregatedValue = _aggregator(properties.Select(p => p.Value));
                    // Устанавливаем результат в результирующее свойство
                    _resultProperty.SetValueAndNotify(aggregatedValue);
                });

                _subscriptions.Add(subscription);
            }

            // Устанавливаем начальное значение на основе текущих значений всех свойств
            var initialValue = _aggregator(properties.Select(p => p.Value));
            _resultProperty.SetValueAndNotify(initialValue);
        }

        /// <summary>
        /// Отписывает все свойства и освобождает ресурсы.
        /// </summary>
        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
        }
    }





    /// <summary>
    /// Модель данных для хранения информации о привязке между `ReactiveProperty` и `BaseField`.
    /// Используется в классе `Binder` для управления привязками и их удалением.
    /// </summary>
    /// <typeparam name="T">Тип данных, с которым работает эта модель.</typeparam>
    internal class BindModel<T>
    {
        public ReactiveProperty<T> ReactiveProperty;
        public BaseField<T> field;
        public EventCallback<ChangeEvent<T>> Delegate;
        public IDisposable Disposable;
    }

    /// <summary>
    /// Модель данных для хранения информации о привязке между `ReactiveProperty` и `BaseField`.
    /// </summary>
    internal class BindModel<TSource, TTarget>
    {
        public ReactiveProperty<TSource> ReactiveProperty;
        public BaseField<TTarget> Field;
        public EventCallback<ChangeEvent<TTarget>> Delegate;
        public IDisposable Disposable;
    }
}