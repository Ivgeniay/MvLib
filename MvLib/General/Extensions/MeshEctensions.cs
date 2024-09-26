using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MvLib
{
    public static class MeshEctensions
    {
        /// <summary>
        /// Создает новый меш, который является копией указанного меша.
        /// </summary>
        /// <param name="mesh">Меш, который нужно скопировать.</param>
        /// <returns>Новый меш, который является копией указанного меша.</returns>
        public static Mesh Copy(this Mesh mesh)
        {
            Mesh copy = new Mesh
            {
                vertices = mesh.vertices,
                triangles = mesh.triangles,
                normals = mesh.normals,
                tangents = mesh.tangents,
                uv = mesh.uv,
                uv2 = mesh.uv2,
                uv3 = mesh.uv3,
                uv4 = mesh.uv4,
                colors = mesh.colors,
                colors32 = mesh.colors32,
                bindposes = mesh.bindposes,
                boneWeights = mesh.boneWeights,
                subMeshCount = mesh.subMeshCount
            };

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                copy.SetTriangles(mesh.GetTriangles(i), i);
            }

            return copy;
        }

        /// <summary>
        /// Поворачивает все вершины меша вокруг указанной оси на заданный угол.
        /// </summary>
        /// <param name="mesh">Меш, вершины которого будут повернуты.</param>
        /// <param name="axis">Ось, вокруг которой будут поворачиваться вершины.</param>
        /// <param name="angle">Угол поворота в градусах.</param>
        public static void RotateVertices(this Mesh mesh, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * vertices[i];
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        } 

        /// <summary>
        /// Добавляет смещение к каждой вершине меша.
        /// </summary>
        /// <param name="mesh">Меш, к вершинам которого будет добавлено смещение.</param>
        /// <param name="offset">Смещение, которое будет добавлено к вершинам.</param>
        public static void ApplyVertexOffset(this Mesh mesh, Vector3 offset)
        {
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += offset;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Изменяет вершины меша в пределах заданного радиуса от указанного центра.
        /// </summary>
        /// <param name="mesh">Меш, вершины которого будут изменены.</param>
        /// <param name="center">Центр области изменения.</param>
        /// <param name="radius">Радиус области изменения.</param>
        /// <param name="modification">Функция, которая будет применена к вершинам внутри области.</param>
        public static void ModifyVerticesInRadius(this Mesh mesh, Vector3 center, float radius, Action<Vector3[]> modification)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] modifiedVertices = (Vector3[])vertices.Clone();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (Vector3.Distance(vertices[i], center) <= radius)
                {
                    modifiedVertices[i] = vertices[i];
                }
            }

            modification(modifiedVertices);
            mesh.vertices = modifiedVertices;
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Добавляет UV координаты к мешу, если они отсутствуют.
        /// </summary>
        /// <param name="mesh">Меш, к которому будут добавлены UV координаты.</param>
        /// <param name="defaultUV">Значение UV координат по умолчанию.</param>
        public static void EnsureUV(this Mesh mesh, Vector2 defaultUV)
        {
            if (mesh.uv.Length == 0)
            {
                Vector2[] uvs = new Vector2[mesh.vertexCount];
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = defaultUV;
                }
                mesh.uv = uvs;
            }
        }

        /// <summary>
        /// Сбрасывает все данные меша, включая вершины, нормали и UV координаты.
        /// </summary>
        /// <param name="mesh">Меш, данные которого будут сброшены.</param>
        public static void ResetMeshData(this Mesh mesh)
        {
            mesh.vertices = new Vector3[0];
            mesh.normals = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Создает меш из предоставленных треугольников.
        /// </summary>
        /// <param name="vertices">Вершины меша.</param>
        /// <param name="triangles">Индексы треугольников.</param>
        /// <returns>Созданный меш.</returns>
        public static Mesh CreateMeshFromTriangles(Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }


        /// <summary>
        /// Удаляет дублирующиеся вершины в меше, сохраняя только уникальные вершины.
        /// </summary>
        /// <param name="mesh">Меш, из которого будут удалены дублирующиеся вершины.</param>
        public static void RemoveDuplicateVertices(this Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            HashSet<Vector3> uniqueVertices = new HashSet<Vector3>(vertices);
            Vector3[] newVertices = uniqueVertices.ToArray();

            Dictionary<Vector3, int> vertexMapping = new Dictionary<Vector3, int>();
            for (int i = 0; i < newVertices.Length; i++)
            {
                vertexMapping[newVertices[i]] = i;
            }

            int[] triangles = mesh.triangles;
            int[] newTriangles = new int[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                newTriangles[i] = vertexMapping[vertices[triangles[i]]];
            }

            mesh.vertices = newVertices;
            mesh.triangles = newTriangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Разделяет меш на несколько частей по заданным индексам.
        /// </summary>
        /// <param name="mesh">Меш, который будет разделен.</param>
        /// <param name="subMeshIndices">Индексы, по которым будет произведено разделение.</param>
        /// <returns>Массив мешей, полученных в результате разделения.</returns>
        public static Mesh[] SplitMesh(this Mesh mesh, int[] subMeshIndices)
        {
            List<Mesh> meshes = new List<Mesh>();
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            foreach (int index in subMeshIndices)
            {
                Mesh newMesh = new Mesh();
                List<Vector3> newVertices = new List<Vector3>();
                List<int> newTriangles = new List<int>();

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    if (triangles[i] == index || triangles[i + 1] == index || triangles[i + 2] == index)
                    {
                        newTriangles.Add(newVertices.IndexOf(vertices[triangles[i]]));
                        newTriangles.Add(newVertices.IndexOf(vertices[triangles[i + 1]]));
                        newTriangles.Add(newVertices.IndexOf(vertices[triangles[i + 2]]));

                        if (!newVertices.Contains(vertices[triangles[i]]))
                            newVertices.Add(vertices[triangles[i]]);
                        if (!newVertices.Contains(vertices[triangles[i + 1]]))
                            newVertices.Add(vertices[triangles[i + 1]]);
                        if (!newVertices.Contains(vertices[triangles[i + 2]]))
                            newVertices.Add(vertices[triangles[i + 2]]);
                    }
                }

                newMesh.vertices = newVertices.ToArray();
                newMesh.triangles = newTriangles.ToArray();
                newMesh.RecalculateNormals();
                newMesh.RecalculateBounds();
                meshes.Add(newMesh);
            }

            return meshes.ToArray();
        }

        /// <summary>
        /// Инвертирует нормали всех вершин меша.
        /// </summary>
        /// <param name="mesh">Меш, у которого будут инвертированы нормали.</param>
        public static void InvertNormals(this Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;
        }
    }
}
