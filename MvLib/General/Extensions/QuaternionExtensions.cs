using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MvLib
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Создает новый кватернион с измененным значением компонента X.
        /// </summary>
        /// <param name="original">Исходный кватернион.</param>
        /// <param name="newX">Новое значение компонента X.</param>
        /// <returns>Новый кватернион с обновленным значением компонента X.</returns>
        public static Quaternion WithX(this Quaternion original, float newX) => new Quaternion(newX, original.y, original.z, original.w);

        /// <summary>
        /// Создает новый кватернион с измененным значением компонента Y.
        /// </summary>
        /// <param name="original">Исходный кватернион.</param>
        /// <param name="newY">Новое значение компонента Y.</param>
        /// <returns>Новый кватернион с обновленным значением компонента Y.</returns>
        public static Quaternion WithY(this Quaternion original, float newY) => new Quaternion(original.x, newY, original.z, original.w);

        /// <summary>
        /// Создает новый кватернион с измененным значением компонента Z.
        /// </summary>
        /// <param name="original">Исходный кватернион.</param>
        /// <param name="newZ">Новое значение компонента Z.</param>
        /// <returns>Новый кватернион с обновленным значением компонента Z.</returns>
        public static Quaternion WithZ(this Quaternion original, float newZ) => new Quaternion(original.x, original.y, newZ, original.w);

        /// <summary>
        /// Создает новый кватернион с измененным значением компонента W.
        /// </summary>
        /// <param name="original">Исходный кватернион.</param>
        /// <param name="newW">Новое значение компонента W.</param>
        /// <returns>Новый кватернион с обновленным значением компонента W.</returns>
        public static Quaternion WithW(this Quaternion original, float newW) => new Quaternion(original.x, original.y, original.z, newW);

        /// <summary>
        /// Возвращает кватернион, который представляет собой результат произведения всех кватернионов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция кватернионов.</param>
        /// <returns>Кватернион, являющийся произведением всех кватернионов в коллекции.</returns>
        public static Quaternion Product(this IEnumerable<Quaternion> source)
        {
            return source.Aggregate(Quaternion.identity, (acc, q) => acc * q);
        }

        /// <summary>
        /// Возвращает кватернион, представляющий собой среднее значение всех кватернионов в коллекции.
        /// </summary>
        /// <param name="source">Коллекция кватернионов.</param>
        /// <returns>Средний кватернион.</returns>
        public static Quaternion Average(this IEnumerable<Quaternion> source)
        {
            var quaternions = source.ToList();
            int count = quaternions.Count;
            if (count == 0) return Quaternion.identity;

            // Суммируем все кватернионы по их компонентам
            var sum = quaternions.Aggregate(Quaternion.identity, (acc, q) => new Quaternion(
                acc.x + q.x,
                acc.y + q.y,
                acc.z + q.z,
                acc.w + q.w
            ));

            // Делим каждую компоненту на количество
            return new Quaternion(
                sum.x / count,
                sum.y / count,
                sum.z / count,
                sum.w / count
            );
        }

        /// <summary>
        /// Возвращает кватернион с минимальным значением компонента.
        /// </summary>
        /// <param name="source">Коллекция кватернионов.</param>
        /// <returns>Кватернион с минимальными компонентами.</returns>
        public static Quaternion Min(this IEnumerable<Quaternion> source)
        {
            return source.Aggregate((acc, q) => new Quaternion(
                Mathf.Min(acc.x, q.x),
                Mathf.Min(acc.y, q.y),
                Mathf.Min(acc.z, q.z),
                Mathf.Min(acc.w, q.w)
            ));
        }

        /// <summary>
        /// Возвращает кватернион с максимальным значением компонента.
        /// </summary>
        /// <param name="source">Коллекция кватернионов.</param>
        /// <returns>Кватернион с максимальными компонентами.</returns>
        public static Quaternion Max(this IEnumerable<Quaternion> source)
        {
            return source.Aggregate((acc, q) => new Quaternion(
                Mathf.Max(acc.x, q.x),
                Mathf.Max(acc.y, q.y),
                Mathf.Max(acc.z, q.z),
                Mathf.Max(acc.w, q.w)
            ));
        }
    }
}
