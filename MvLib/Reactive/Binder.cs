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
    public class Binder<T> : IDisposable
    {
        private List<BindModel<T>> bindModels = new List<BindModel<T>>();


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
            else
            {
                UnBind(reactiveProp, field);
            }

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
}