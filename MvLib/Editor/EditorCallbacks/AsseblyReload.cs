using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MvLib
{ 

    [InitializeOnLoad]
    internal static class AsseblyReload
    {
        static List<IAssemblyCallback> assemblyCallbackInstaces = new List<IAssemblyCallback>();

        static AsseblyReload()
        { 
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload; 
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }
        
        internal static void OnBeforeAssemblyReload()
        {
            List<Assembly> assemblies = new List<Assembly>();
            try
            {
                Assembly unityAssembly = Assembly.Load("Assembly-CSharp");
                if (unityAssembly != null) assemblies.Add(unityAssembly);
            }
            catch {}
            try
            {
                Assembly unityEditorAssembly = Assembly.Load("Assembly-CSharp-Editor");
                if (unityEditorAssembly != null) assemblies.Add(unityEditorAssembly);
            }
            catch { }

            assemblies.Add(Assembly.GetExecutingAssembly());
            List<Type> typesCallbacks = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                typesCallbacks.AddRange(
                    assembly
                        .GetTypes()
                        .Where(e => !e.IsInterface && typeof(IAssemblyCallback)
                        .IsAssignableFrom(e))
                    );
            }

            assemblyCallbackInstaces = CollectCallbacksFromScene();
            assemblyCallbackInstaces.ForEach(e => e.OnBeforeAssemblyReload());
        }

        internal static void OnAfterAssemblyReload()
        { 
            assemblyCallbackInstaces.ForEach(e => e.OnAfterAssemblyReload());
        }

        private static List<IAssemblyCallback> CollectCallbacksFromScene()
        {
            List<IAssemblyCallback> callbackInstances = new List<IAssemblyCallback>(); 
            Scene currentScene = SceneManager.GetActiveScene(); 
            GameObject[] rootObjects = currentScene.GetRootGameObjects(); 
            foreach (GameObject rootObject in rootObjects)
            { 
                IAssemblyCallback[] callbacks = rootObject.GetComponentsInChildren<IAssemblyCallback>(true); 
                callbackInstances.AddRange(callbacks);
            } 
            return callbackInstances;
        }
    }
}
