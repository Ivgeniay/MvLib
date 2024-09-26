using System;
using System.Collections.Generic;
using UnityEngine;

namespace MvLib
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Получает компонент указанного типа на объекте или добавляет его, если он отсутствует.
        /// Это удобно для гарантированного наличия компонента на объекте, не проверяя его наличие вручную.
        /// </summary>
        /// <typeparam name="T">Тип компонента, который нужно получить или добавить.</typeparam>
        /// <param name="gameObject">Игровой объект, на котором проверяется наличие компонента или на который добавляется компонент.</param>
        /// <returns>Компонент указанного типа, если он уже существует, или новый компонент, добавленный к игровому объекту.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        { 
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Копирует все компоненты указанного типа из одного игрового объекта в другой.
        /// Это может быть полезно для копирования настроек и данных между игровыми объектами в редакторе Unity.
        /// </summary>
        /// <typeparam name="T">Тип компонентов, которые нужно скопировать.</typeparam>
        /// <param name="source">Игровой объект, из которого копируются компоненты.</param>
        /// <param name="destination">Игровой объект, на который добавляются скопированные компоненты.</param>
        /// <remarks>
        /// Этот метод использует функционал `UnityEditorInternal.ComponentUtility`, который доступен только в редакторе Unity.
        /// </remarks>
        public static void CopyComponents<T>(this GameObject source, GameObject destination) where T : Component
        {
            if (destination == null)
            {
                Debug.LogError($"Destination GameObject is null (from {nameof(CopyComponents)})");
                return;
            }

            T[] sourceComponents = source.GetComponents<T>();
            foreach (T component in sourceComponents)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destination);
            }
        }

        /// <summary>
        /// Устанавливает активное состояние для всех дочерних объектов текущего игрового объекта.
        /// </summary>
        /// <param name="gameObject">Игровой объект, дочерние объекты которого будут изменены.</param>
        /// <param name="active">Активное состояние для дочерних объектов.</param>
        public static void SetActiveInChildren(this GameObject gameObject, bool active)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Включает или отключает все коллайдеры на игровом объекте.
        /// </summary>
        /// <param name="gameObject">Игровой объект, коллайдеры которого будут включены/отключены.</param>
        /// <param name="enabled">Состояние включения/отключения коллайдеров.</param>
        public static void SetCollidersEnabled(this GameObject gameObject, bool enabled)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = enabled;
            }
        }

        /// <summary>
        /// Добавляет компонент указанного типа к игровому объекту, если он ещё не добавлен.
        /// Если компонент уже существует, обновляет его настройки с помощью указанного делегата.
        /// </summary>
        /// <typeparam name="T">Тип компонента, который нужно добавить или обновить.</typeparam>
        /// <param name="gameObject">Игровой объект, к которому добавляется компонент.</param>
        /// <param name="configure">Делегат для настройки компонента, если он уже существует.</param>
        /// <returns>Компонент указанного типа.</returns>
        public static T AddOrUpdateComponent<T>(this GameObject gameObject, Action<T> configure = null) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            configure?.Invoke(component);
            return component;
        }

        /// <summary>
        /// Удаляет все компоненты указанного типа из игрового объекта и его дочерних объектов.
        /// </summary>
        /// <typeparam name="T">Тип компонентов, которые нужно удалить.</typeparam>
        /// <param name="gameObject">Игровой объект, из которого удаляются компоненты.</param>
        public static void RemoveComponents<T>(this GameObject gameObject) where T : Component
        {
            T[] components = gameObject.GetComponentsInChildren<T>(true);
            foreach (T component in components)
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        /// <summary>
        /// Сбрасывает позицию, поворот и масштаб игрового объекта.
        /// </summary>
        /// <param name="gameObject">Игровой объект, который нужно сбросить.</param>
        public static void ResetTransform(this GameObject gameObject)
        {
            gameObject.transform.ResetTransform();
        }

        /// <summary>
        /// Находит компонент указанного типа среди дочерних объектов игрового объекта.
        /// Использует имя компонента для поиска.
        /// </summary>
        /// <typeparam name="T">Тип компонента, который нужно найти.</typeparam>
        /// <param name="gameObject">Игровой объект, дочерние объекты которого будут проверяться.</param>
        /// <param name="name">Имя компонента, который нужно найти.</param>
        /// <returns>Компонент указанного типа, если найден, иначе null.</returns>
        public static T FindComponentInChildrenByName<T>(this GameObject gameObject, string name) where T : Component
        {
            T[] components = gameObject.GetComponentsInChildren<T>();
            foreach (T component in components)
            {
                if (component.gameObject.name == name)
                {
                    return component;
                }
            }
            return null;
        }

        /// <summary>
        /// Активирует все игровые объекты в сцене, которые имеют указанный тег.
        /// </summary>
        /// <param name="tag">Тег, который должен иметься у игровых объектов для их активации.</param>
        public static void ActivateObjectsWithTag(string tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsWithTag)
            {
                obj.SetActive(true);
            }
        }

        /// <summary>
        /// Создает клон текущего игрового объекта, включая его дочерние объекты.
        /// </summary>
        /// <param name="gameObject">Игровой объект, который нужно клонировать.</param>
        /// <returns>Созданный клон игрового объекта.</returns>
        public static GameObject CloneWithChildren(this GameObject gameObject)
        {
            return UnityEngine.Object.Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
        }

        /// <summary>
        /// Сохраняет состояние активного состояния игрового объекта и его дочерних объектов.
        /// </summary>
        /// <param name="gameObject">Игровой объект, состояние которого нужно сохранить.</param>
        /// <returns>Словарь, содержащий состояние активного состояния каждого объекта.</returns>
        public static Dictionary<GameObject, bool> SaveActiveState(this GameObject gameObject)
        {
            Dictionary<GameObject, bool> states = new Dictionary<GameObject, bool>();
            foreach (Transform child in gameObject.transform)
            {
                states[child.gameObject] = child.gameObject.activeSelf;
            }
            return states;
        }

        /// <summary>
        /// Восстанавливает сохраненное состояние активного состояния для игрового объекта и его дочерних объектов.
        /// </summary>
        /// <param name="gameObject">Игровой объект, для которого нужно восстановить состояние.</param>
        /// <param name="states">Словарь, содержащий сохраненное состояние активного состояния каждого объекта.</param>
        public static void RestoreActiveState(this GameObject gameObject, Dictionary<GameObject, bool> states)
        {
            foreach (var kvp in states)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.SetActive(kvp.Value);
                }
            }
        }

        /// <summary>
        /// Перемещает все дочерние объекты текущего игрового объекта на новый родительский объект.
        /// </summary>
        /// <param name="gameObject">Игровой объект, дочерние объекты которого будут перемещены.</param>
        /// <param name="newParent">Новый родительский объект, к которому будут перемещены дочерние объекты.</param>
        public static void MoveChildrenToNewParent(this GameObject gameObject, GameObject newParent)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.SetParent(newParent.transform);
            }
        }
    }
}
