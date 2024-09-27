using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MvLib
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Вычисляет точку пересечения луча от позиции мыши с плоскостью, заданной тремя точками.
        /// Проверяет, коллинеарны ли точки, и если они коллинеарны, возвращает предупреждение.
        /// </summary>
        /// <param name="mousePositionOnGui">Позиция мыши в GUI координатах.</param>
        /// <param name="pointA">Первая точка плоскости.</param>
        /// <param name="pointB">Вторая точка плоскости.</param>
        /// <param name="pointC">Третья точка плоскости.</param>
        /// <returns>Точка пересечения на плоскости или Vector3.zero, если пересечения нет.</returns>
        public static Vector3 GetPointOnPlane(this Vector2 mousePositionOnGui, Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            if (pointA.AreCollinear(pointB, pointC))
            {
                Debug.LogWarning("Точки коллинеарны, не могут описывать плоскость.");
                return Vector3.zero;
            } 
            Vector3 normal = Vector3.Cross(pointB - pointA, pointC - pointA).normalized; 
            Vector3 center = (pointA + pointB + pointC) / 3; 
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePositionOnGui);
            Plane plane = new Plane(normal, center);
            float enter;

            if (plane.Raycast(mouseRay, out enter))
            {
                Vector3 intersectionPoint = mouseRay.GetPoint(enter);
                return intersectionPoint;
            }

            return Vector3.zero; // Если нет пересечения, возвращаем нулевую точку
        }

        /// <summary>
        /// Проверяет, являются ли три точки коллинеарными, то есть лежат ли они на одной прямой.
        /// Использует векторное произведение для проверки.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <param name="c">Третья точка.</param>
        /// <returns>Возвращает true, если три точки коллинеарны.</returns>
        public static bool AreCollinear(this Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;

            return Mathf.Abs(Vector3.Cross(ab, ac).magnitude) < Mathf.Epsilon;
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри треугольника, заданного тремя вершинами.
        /// Использует метод сравнения сторон для определения положения точки.
        /// </summary>
        /// <param name="point">Точка, которую нужно проверить.</param>
        /// <param name="v0">Первая вершина треугольника.</param>
        /// <param name="v1">Вторая вершина треугольника.</param>
        /// <param name="v2">Третья вершина треугольника.</param>
        /// <returns>Возвращает true, если точка находится внутри треугольника.</returns>
        public static bool IsPointInTriangle(this Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            // Проверка на стороне треугольника
            return IsSameSide(point, v0, v1, v2) &&
                   IsSameSide(point, v1, v0, v2) &&
                   IsSameSide(point, v2, v0, v1);
        }

        /// <summary>
        /// Определяет, находится ли точка внутри четырехугольника, заданного четырьмя вершинами.
        /// Используется метод сравнения сторон для определения положения точки.
        /// </summary>
        /// <param name="point">Точка, которую нужно проверить.</param>
        /// <param name="v0">Первая вершина четырехугольника.</param>
        /// <param name="v1">Вторая вершина четырехугольника.</param>
        /// <param name="v2">Третья вершина четырехугольника.</param>
        /// <param name="v3">Четвертая вершина четырехугольника.</param>
        /// <returns>Возвращает true, если точка находится внутри четырехугольника.</returns>
        public static bool IsPointInSquare(this Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        { 
            return IsSameSide(point, v0, v1, v2) &&
                   IsSameSide(point, v1, v2, v3) &&
                   IsSameSide(point, v2, v3, v0) &&
                   IsSameSide(point, v3, v0, v1);
        }
        private static bool IsSameSide(Vector3 p1, Vector3 p2, Vector3 v1, Vector3 v2)
        {
            Vector3 cp1 = Vector3.Cross(v2 - v1, p1 - v1);
            Vector3 cp2 = Vector3.Cross(v2 - v1, p2 - v1);
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        /// <summary>
        /// Возвращает нормализованный вектор, если его длина больше 0. 
        /// Если вектор равен нулю, возвращает Vector3.zero.
        /// Используется для предотвращения деления на ноль при нормализации.
        /// </summary>
        /// <param name="vector">Исходный вектор для нормализации.</param>
        /// <returns>Нормализованный вектор или Vector3.zero, если исходный вектор равен нулю.</returns>
        public static Vector3 SafeNormalize(this Vector3 vector)
        {
            if (vector.sqrMagnitude == 0) return Vector3.zero;
            return vector.normalized;
        }

        /// <summary>
        /// Возвращает произвольный перпендикулярный вектор для данного вектора.
        /// Если вектор направлен вдоль оси Y, возвращает вектор вдоль оси X.
        /// В остальных случаях возвращает нормализованный перпендикулярный вектор.
        /// </summary>
        /// <param name="vector">Вектор, для которого нужно найти перпендикулярный вектор.</param>
        /// <returns>Нормализованный вектор, перпендикулярный исходному вектору.</returns>
        public static Vector3 Perpendicular(this Vector3 vector)
        {
            if (Mathf.Approximately(vector.x, 0) && Mathf.Approximately(vector.z, 0))
                return new Vector3(1, 0, 0);

            return new Vector3(-vector.y, vector.x, vector.z).SafeNormalize();
        }

        /// <summary>
        /// Возвращает вектор, противоположный данному, то есть с компонентами, умноженными на -1.
        /// Полезно для инверсии направления вектора.
        /// </summary>
        /// <param name="vector">Исходный вектор, направление которого нужно инвертировать.</param>
        /// <returns>Противоположный вектор, где каждая компонента умножена на -1.</returns>
        public static Vector3 Opposite(this Vector3 vector) => -vector;

        /// <summary>
        /// Округляет компоненты вектора до ближайшего кратного заданного значения.
        /// Это может быть полезно для "квантования" координат вектора в сетке с определённым шагом (q).
        /// </summary>
        /// <param name="vector">Исходный вектор, который нужно квантовать.</param>
        /// <param name="q">Шаг квантования. Определяет, к каким значениям будут округлены компоненты вектора.</param>
        /// <returns>Новый вектор с округлёнными компонентами, каждая из которых кратна q.</returns>
        public static Vector3 Quantize(this Vector3 vector, float q)
        {
            Vector3 result = vector;
            result.x = Mathf.Round(vector.x / q) * q;
            result.y = Mathf.Round(vector.y / q) * q;
            result.z = Mathf.Round(vector.z / q) * q;
            return result;
        }

        /// <summary>
        /// Проверяет, находится ли вектор внутри указанных границ (min и max).
        /// </summary>
        /// <param name="vector">Вектор для проверки.</param>
        /// <param name="min">Минимальная граница.</param>
        /// <param name="max">Максимальная граница.</param>
        /// <returns>Возвращает true, если вектор находится в пределах границ.</returns>
        public static bool IsWithinBounds(this Vector3 vector, Vector3 min, Vector3 max)
        {
            return vector.x >= min.x && vector.x <= max.x &&
                   vector.y >= min.y && vector.y <= max.y &&
                   vector.z >= min.z && vector.z <= max.z;
        }

        /// <summary>
        /// Проверяет, является ли вектор ненулевым, то есть содержит ли хотя бы одна из его компонент ненулевое значение.
        /// </summary>
        /// <param name="vector">Вектор для проверки.</param>
        /// <returns>Возвращает true, если хотя бы одна из компонент вектора не равна нулю.</returns>
        public static bool IsNonZero(this Vector3 vector)
        {
            return vector.x != 0 || vector.y != 0 || vector.z != 0;
        }

        /// <summary>
        /// Преобразует HSV представление цвета (тон, насыщенность, яркость) в RGB цвет.
        /// </summary>
        /// <param name="hsv">Вектор, представляющий цвет в формате HSV: X - Hue (тон), Y - Saturation (насыщенность), Z - Value (яркость).</param>
        /// <returns>Цвет в формате RGB.</returns>
        public static Color HSVToColor(this Vector3 hsv)
        {
            float h = hsv.x;
            float s = hsv.y;
            float v = hsv.z;

            float c = v * s; // Chroma
            float x = c * (1 - Mathf.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float r = 0, g = 0, b = 0;

            if (h >= 0 && h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (h >= 300 && h < 360)
            {
                r = c; g = 0; b = x;
            }

            return new Color(r + m, g + m, b + m);
        }

        /// <summary>
        /// Возвращает новый вектор с измененным значением X, сохраняя текущие значения Y и Z.
        /// </summary>
        /// <param name="original">Исходный вектор.</param>
        /// <param name="newX">Новое значение для компонента X.</param>
        /// <returns>Новый вектор с обновленным X.</returns>
        public static Vector3 WithX(this Vector3 original, float newX) => new Vector3(newX, original.y, original.z);
        /// <summary>
        /// Возвращает новый вектор с измененным значением Y, сохраняя текущие значения X и Z.
        /// </summary>
        /// <param name="original">Исходный вектор.</param>
        /// <param name="newY">Новое значение для компонента Y.</param>
        /// <returns>Новый вектор с обновленным Y.</returns>
        public static Vector3 WithY(this Vector3 original, float newY) => new Vector3(original.x, newY, original.z);
        /// <summary>
        /// Возвращает новый вектор с измененным значением Z, сохраняя текущие значения X и Y.
        /// </summary>
        /// <param name="original">Исходный вектор.</param>
        /// <param name="newZ">Новое значение для компонента Z.</param>
        /// <returns>Новый вектор с обновленным Z.</returns>
        public static Vector3 WithZ(this Vector3 original, float newZ) => new Vector3(original.x, original.y, newZ);


        #region Collection
        /// <summary>
        /// Возвращает сумму всех векторов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция векторов.</param>
        /// <returns>Вектор, являющийся суммой всех векторов в коллекции.</returns>
        public static Vector3 Sum(this IEnumerable<Vector3> source)
        {
            return source.Aggregate(Vector3.zero, (acc, v) => acc + v);
        }

        /// <summary>
        /// Возвращает среднее значение всех векторов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция векторов.</param>
        /// <returns>Средний вектор.</returns>
        public static Vector3 Average(this IEnumerable<Vector3> source)
        {
            int count = source.Count();
            return count > 0 ? source.Sum() / count : Vector3.zero;
        }

        /// <summary>
        /// Возвращает вектор с минимальными значениями компонентов X, Y и Z среди всех векторов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция векторов.</param>
        /// <returns>Вектор, содержащий минимальные компоненты среди всех векторов.</returns>
        public static Vector3 Min(this IEnumerable<Vector3> source)
        {
            return source.Any() ? source.Aggregate((acc, v) => Vector3.Min(acc, v)) : Vector3.zero;
        }

        /// <summary>
        /// Возвращает вектор с максимальными значениями компонентов X, Y и Z среди всех векторов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция векторов.</param>
        /// <returns>Вектор, содержащий максимальные компоненты среди всех векторов.</returns>
        public static Vector3 Max(this IEnumerable<Vector3> source)
        {
            return source.Any() ? source.Aggregate((acc, v) => Vector3.Max(acc, v)) : Vector3.zero;
        }
        #endregion
    }
}
