using System.Collections;
using UnityEngine;

namespace MvLib 
{
    /// <summary>
    /// Синглтон-класс для управления корутинами в Unity. 
    /// Обеспечивает создание и запуск корутин, а также гарантирует существование только одного экземпляра класса в проекте.
    /// </summary>
    public class Routine : MonoBehaviour
    {
        private static Routine instance;
        public static Routine Instance { get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<Routine>();
                    if (instance == null)
                    {
                        instance = new GameObject("[Routine]").AddComponent<Routine>();
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
                return instance;
            }
        }
    }
}
