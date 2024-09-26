using System.Collections.Generic; 
using UnityEngine;
using System;

namespace MvLib
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Возвращает перечисление всех дочерних объектов текущего трансформа, включая их дочерние объекты, рекурсивно.
        /// </summary>
        /// <param name="source">Трансформ, из которого будут получены все дочерние объекты.</param>
        /// <returns>Перечисление всех дочерних объектов, включая их дочерние объекты.</returns>
        public static IEnumerable<Transform> GetAllTransformChilds(this Transform source)
        { 
            var count = source.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = source.GetChild(i);
                yield return child;
                if (child.childCount > 0)
                {
                    foreach (var e in GetAllTransformChilds(child))
                        yield return e;
                }
            }
        }

        /// <summary>
        /// Клонирует иерархию дочерних объектов из исходного трансформа в целевой трансформ.
        /// </summary>
        /// <param name="source">Исходный трансформ, из которого будет клонироваться иерархия.</param>
        /// <param name="destination">Целевой трансформ, в который будет добавлена клонированная иерархия.</param>
        /// <param name="newTransforms">Список, в который будут добавлены новые трансформы.</param>
        public static void CloneHyerarhy(this Transform source, Transform destination, ref List<Transform> newTransforms)
        {
            foreach (Transform child in source)
            {
                if (newTransforms.Contains(source)) continue;

                GameObject newChild = new GameObject($"{child.name}");
                newTransforms.Add(newChild.transform);

                newChild.transform.SetParent(destination);
                newChild.transform.localPosition = child.localPosition;
                newChild.transform.localRotation = child.localRotation;
                newChild.transform.localScale = child.localScale;

                child.CloneHyerarhy(newChild.transform, ref newTransforms);
            }
        }

        /// <summary>
        /// Проверяет, является ли текущий трансформ дочерним для указанного трансформа.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на принадлежность к родителю.</param>
        /// <param name="parent">Трансформ-родитель, к которому проверяется принадлежность.</param>
        /// <returns>Возвращает true, если текущий трансформ является дочерним для указанного родителя, иначе false.</returns>
        public static bool IsChildOf(this Transform source, Transform parent)
        {
            if (source == parent) return true;
            if (source.parent == null) return false;
            return source.parent.IsChildOf(parent);
        }

        /// <summary>
        /// Проверяет, является ли текущий трансформ родителем указанного трансформа.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на наличие дочернего трансформа.</param>
        /// <param name="child">Трансформ, который проверяется на принадлежность к родителю.</param>
        /// <returns>Возвращает true, если текущий трансформ является родителем указанного трансформа, иначе false.</returns>
        public static bool IsParentOf(this Transform source, Transform child)
        {
            if (source == child) return true;
            if (child.parent == null) return false;
            return source.IsParentOf(child.parent);
        }

        /// <summary>
        /// Устанавливает указанный слой для текущего трансформа и всех его дочерних объектов рекурсивно.
        /// </summary>
        /// <param name="source">Трансформ, для которого устанавливается слой.</param>
        /// <param name="layer">Идентификатор слоя, который будет установлен.</param>
        public static void SetLayerRecursively(this Transform source, int layer)
        {
            source.gameObject.layer = layer;
            foreach (Transform child in source)
            {
                child.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Проверяет, видим ли текущий трансформ из указанной камеры на основе границ рендерера.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на видимость.</param>
        /// <param name="camera">Камера, из которой проверяется видимость.</param>
        /// <returns>Возвращает true, если трансформ видим из указанной камеры, иначе false.</returns>
        public static bool IsVisibleFrom(this Transform source, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, source.GetComponent<Renderer>().bounds);
        }

        /// <summary>
        /// Проверяет, видим ли текущий трансформ из указанной камеры на основе границ рендерера с учетом смещения.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на видимость.</param>
        /// <param name="camera">Камера, из которой проверяется видимость.</param>
        /// <param name="offset">Смещение границ для расширения области проверки видимости.</param>
        /// <returns>Возвращает true, если трансформ видим из указанной камеры с учетом смещения, иначе false.</returns>
        public static bool IsVisibleFrom(this Transform source, Camera camera, float offset = 1f)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            Bounds bounds = source.GetComponent<Renderer>().bounds;
            bounds.Expand(offset);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }

        /// <summary>
        /// Проверяет, находится ли текущий трансформ в пределах указанного расстояния от другого трансформа.
        /// </summary>
        /// <param name="source">Трансформ, для которого проверяется расстояние.</param>
        /// <param name="target">Целевой трансформ, от которого проверяется расстояние.</param>
        /// <param name="distance">Максимальное расстояние, при котором проверяется близость.</param>
        /// <returns>Возвращает true, если текущий трансформ находится в пределах указанного расстояния от целевого трансформа, иначе false.</returns>
        public static bool IsNearFrom(this Transform source, Transform target, float distance) => 
            Vector3.Distance(source.position, target.position) <= distance;

        /// <summary>
        /// Проверяет, является ли текущий трансформ null.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на null.</param>
        /// <returns>Возвращает true, если трансформ равен null, иначе false.</returns>
        public static bool IsNull(this Transform source) => source == null;

        /// <summary>
        /// Проверяет, не является ли текущий трансформ null.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на null.</param>
        /// <returns>Возвращает true, если трансформ не равен null, иначе false.</returns>
        public static bool IsNotNull(this Transform source) => source != null;

        /// <summary>
        /// Проверяет, является ли текущий трансформ тем же, что и указанный трансформ.
        /// </summary>
        /// <param name="source">Текущий трансформ.</param>
        /// <param name="target">Трансформ для сравнения.</param>
        /// <returns>Возвращает true, если текущий трансформ равен указанному, иначе false.</returns>
        public static bool IsThis(this Transform source, Transform target) => source == target;

        /// <summary>
        /// Проверяет, не является ли текущий трансформ тем же, что и указанный трансформ.
        /// </summary>
        /// <param name="source">Текущий трансформ.</param>
        /// <param name="target">Трансформ для сравнения.</param>
        /// <returns>Возвращает true, если текущий трансформ не равен указанному, иначе false.</returns>
        public static bool IsNotThis(this Transform source, Transform target) => source != target;

        /// <summary>
        /// Проверяет, является ли игровой объект текущего трансформа null.
        /// </summary>
        /// <param name="source">Трансформ, чей игровой объект проверяется на null.</param>
        /// <returns>Возвращает true, если игровой объект равен null, иначе false.</returns>
        public static bool IsGameObjectNull(this Transform source) => source.gameObject == null;

        /// <summary>
        /// Проверяет, не является ли игровой объект текущего трансформа null.
        /// </summary>
        /// <param name="source">Трансформ, чей игровой объект проверяется на null.</param>
        /// <returns>Возвращает true, если игровой объект не равен null, иначе false.</returns>
        public static bool IsGameObjectNotNull(this Transform source) => source.gameObject != null;

        /// <summary>
        /// Проверяет, является ли родитель текущего трансформа null.
        /// </summary>
        /// <param name="source">Трансформ, чей родитель проверяется на null.</param>
        /// <returns>Возвращает true, если родитель равен null, иначе false.</returns>
        public static bool IsParentNull(this Transform source) => source.parent == null;

        /// <summary>
        /// Проверяет, не является ли родитель текущего трансформа null.
        /// </summary>
        /// <param name="source">Трансформ, чей родитель проверяется на null.</param>
        /// <returns>Возвращает true, если родитель не равен null, иначе false.</returns>
        public static bool IsParentNotNull(this Transform source) => source.parent != null;

        /// <summary>
        /// Проверяет, имеет ли текущий трансформ дочерние объекты.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на наличие дочерних объектов.</param>
        /// <returns>Возвращает true, если у трансформа нет дочерних объектов, иначе false.</returns>
        public static bool IsChildNull(this Transform source) => source.childCount == 0;

        /// <summary>
        /// Проверяет, имеет ли текущий трансформ дочерние объекты.
        /// </summary>
        /// <param name="source">Трансформ, который проверяется на наличие дочерних объектов.</param>
        /// <returns>Возвращает true, если у трансформа есть дочерние объекты, иначе false.</returns>
        public static bool IsChildNotNull(this Transform source) => source.childCount != 0;

        /// <summary>
        /// Сбрасывает позицию, поворот и масштаб объекта Transform.
        /// </summary>
        /// <param name="source">Transform игрового объекта, который нужно сбросить.</param>
        public static void ResetTransform(this Transform source)
        {
            source.position = Vector3.zero;
            source.rotation = Quaternion.identity;
            source.localScale = Vector3.one;
        }


        /// <summary>
        /// Сохраняет текущее состояние позиции, поворота и масштаба игрового объекта.
        /// </summary>
        /// <param name="source">Игровой объект, состояние которого нужно сохранить.</param>
        /// <returns>Структура с сохраненным состоянием позиции, поворота и масштаба.</returns>
        public static TransformState SaveTransformState(this Transform source)
        {
            return new TransformState
            {
                Position = source.position,
                Rotation = source.rotation,
                Scale = source.localScale
            };
        }

        /// <summary>
        /// Восстанавливает сохраненное состояние позиции, поворота и масштаба для игрового объекта.
        /// </summary>
        /// <param name="source">Игровой объект, состояние которого нужно восстановить.</param>
        /// <param name="state">Сохраненное состояние позиции, поворота и масштаба.</param>
        public static void RestoreTransformState(this Transform source, TransformState state)
        {
            source.position = state.Position;
            source.rotation = state.Rotation;
            source.localScale = state.Scale;
        }
    }

    public struct TransformState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}
