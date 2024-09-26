using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace MvLib
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Устанавливает видимость элемента `VisualElement` на основе логического значения.
        /// Если значение `true`, элемент становится видимым, если `false`, элемент скрывается.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого устанавливается видимость.</param>
        /// <param name="value">Значение, указывающее, должен ли элемент быть видимым (true) или скрытым (false).</param>
        public static void SetVisibility(this VisualElement root, bool value)
        {
            root.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #region Animation

        /// <summary>
        /// Запускает анимацию плавного появления элемента `VisualElement`, увеличивая его непрозрачность с 0 до 1.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого запускается анимация появления.</param>
        /// <param name="fadeDuration">Продолжительность анимации в секундах. По умолчанию 2.5 секунды.</param>
        /// <param name="onComplete">Функция, которая будет вызвана после завершения анимации.</param>
        public static void StartFadeInAnimation(this VisualElement root, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            root.style.opacity = 0f;

            root.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                root.style.opacity = progress;
                if (progress >= 1f)
                {
                    root.style.opacity = 1f;
                    onComplete?.Invoke();
                }
            }).Until(() => root.style.opacity.value >= 1f);
        }

        /// <summary>
        /// Запускает анимацию плавного исчезновения элемента `VisualElement`, уменьшая его непрозрачность с 1 до 0.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого запускается анимация исчезновения.</param>
        /// <param name="fadeDuration">Продолжительность анимации в секундах. По умолчанию 2.5 секунды.</param>
        /// <param name="onComplete">Функция, которая будет вызвана после завершения анимации.</param>
        public static void StartFadeOutAnimation(this VisualElement root, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            root.style.opacity = 1f;

            root.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                root.style.opacity = 1f - progress;
                if (progress >= 1f)
                {
                    root.style.opacity = 0f;
                    onComplete?.Invoke();
                }
            }).Until(() => root.style.opacity.value <= 0f);
        }

        /// <summary>
        /// Запускает анимацию раскрытия элемента `VisualElement`, увеличивая его высоту с 0 до заданного значения.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого запускается анимация раскрытия.</param>
        /// <param name="originalHeight">Исходная высота элемента, до анимации раскрытия.</param>
        /// <param name="fadeDuration">Продолжительность анимации в секундах. По умолчанию 2.5 секунды.</param>
        /// <param name="onComplete">Функция, которая будет вызвана после завершения анимации.</param>
        public static void StartUnfoldAnimation(this VisualElement root, float originalHeight, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            root.style.height = 0;

            root.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                root.style.height = Mathf.Lerp(0, originalHeight, progress);

                if (progress >= 1f)
                {
                    root.style.height = originalHeight;
                    onComplete?.Invoke();
                }
            }).Until(() => root.style.height == originalHeight);
        }

        /// <summary>
        /// Запускает анимацию сворачивания элемента `VisualElement`, уменьшая его высоту с исходного значения до 0.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого запускается анимация сворачивания.</param>
        /// <param name="originalHeight">Исходная высота элемента, до анимации сворачивания.</param>
        /// <param name="fadeDuration">Продолжительность анимации в секундах. По умолчанию 2.5 секунды.</param>
        /// <param name="onComplete">Функция, которая будет вызвана после завершения анимации.</param>
        public static void StartFoldAnimation(this VisualElement root, float originalHeight, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;

            root.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                root.style.height = Mathf.Lerp(originalHeight, 0, progress);

                if (progress >= 1f)
                {
                    root.style.height = 0;
                    onComplete?.Invoke();
                }
            }).Until(() => root.style.height == 0);
        }
        #endregion


        #region Find

        /// <summary>
        /// Ищет и возвращает все дочерние элементы типа `T` в иерархии элементов `VisualElement`.
        /// </summary>
        /// <typeparam name="T">Тип дочернего элемента, который нужно найти. Должен быть производным от `VisualElement`.</typeparam>
        /// <param name="root">Корневой элемент, с которого начинается поиск.</param>
        /// <returns>Перечисление элементов типа `T`, найденных в иерархии.</returns>
        public static IEnumerable<T> FindElementsOfType<T>(this VisualElement root) where T : VisualElement
        {
            foreach (var child in root.Children())
            {
                if (child is T typedElement)
                {
                    yield return typedElement;
                }
                if (child.childCount > 0)
                {
                    foreach (var element in child.FindElementsOfType<T>())
                    {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>
        /// Ищет и возвращает первый дочерний элемент, удовлетворяющий заданному условию.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, в котором выполняется поиск.</param>
        /// <param name="predicate">Функция-предикат, которая определяет условие для поиска.</param>
        /// <returns>Первый найденный дочерний элемент, удовлетворяющий условию, или `null`, если такой элемент не найден.</returns>
        public static VisualElement FindChild(this VisualElement root, Func<VisualElement, bool> predicate)
        {
            if (root == null) return null;
            foreach (var child in root.Children())
            {
                if (predicate(child)) return child;
                var result = child.FindChild(predicate);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Ищет и возвращает все дочерние элементы, удовлетворяющие заданному условию.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, в котором выполняется поиск.</param>
        /// <param name="predicate">Функция-предикат, которая определяет условие для поиска.</param>
        /// <returns>Перечисление дочерних элементов, удовлетворяющих условию.</returns>
        public static IEnumerable<VisualElement> FindChildren(this VisualElement root, Func<VisualElement, bool> predicate)
        {
            if (root == null) yield break;
            foreach (var child in root.Children())
            {
                if (predicate(child)) yield return child;
                foreach (var descendant in child.FindChildren(predicate))
                {
                    yield return descendant;
                }
            }
        }

        #endregion

        #region Style

        /// <summary>
        /// Добавляет класс к элементу `VisualElement`, если он ещё не присутствует.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, к которому добавляется класс.</param>
        /// <param name="className">Имя класса, который нужно добавить.</param>
        public static void AddClass(this VisualElement root, string className)
        {
            if (!root.ClassListContains(className))
                root.AddToClassList(className);
        }

        /// <summary>
        /// Удаляет класс из элемента `VisualElement`, если он присутствует.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, из которого удаляется класс.</param>
        /// <param name="className">Имя класса, который нужно удалить.</param>
        public static void RemoveClass(this VisualElement root, string className)
        {
            if (root.ClassListContains(className))
                root.RemoveFromClassList(className);
        }

        /// <summary>
        /// Включает или выключает класс у элемента `VisualElement`, в зависимости от логического значения.
        /// </summary>
        /// <param name="root">Элемент `VisualElement`, для которого выполняется переключение класса.</param>
        /// <param name="className">Имя класса, который нужно переключить.</param>
        /// <param name="add">Если `true`, класс добавляется; если `false`, класс удаляется.</param>
        public static void ToggleClass(this VisualElement root, string className, bool add)
        {
            if (add) root.AddClass(className);
            else root.RemoveClass(className);
        }
        #endregion

        /// <summary>
        /// Удаляет все дочерние элементы из элемента `VisualElement`.
        /// </summary>
        /// <param name="parent">Элемент `VisualElement`, из которого удаляются все дочерние элементы.</param>
        public static void RemoveAllChildren(this VisualElement parent)
        {
            int removalCount = parent.Q<VisualElement>().childCount;
            for (int i = 0; i < removalCount; i++)
            {
                parent.Q<VisualElement>().RemoveFromHierarchy();
            } 
        } 
    }
}
