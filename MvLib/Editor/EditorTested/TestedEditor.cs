using System.Collections.Generic;
using UnityEditor.UIElements; 
using UnityEngine.UIElements;
using System.Diagnostics;
using System.Reflection; 
using UnityEditor;
using UnityEngine;
using System;
using MvLib.Reactive;

namespace MvLib.Testing.Inspector
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class TestedEditor : Editor
    {
        protected const string BASE_INSPECTOR_GUI = "BASE";
        protected const string TEST_INSPECTOR_GUI = "TEST";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            VisualElement baseWrap = new VisualElement();
            baseWrap.name = BASE_INSPECTOR_GUI;
            root.Add(baseWrap);

            if (target is not ITestableInInspector testableTarget)
            {
                DrawInspectorWithCustomProperties(baseWrap);
                return root; 
            }


            VisualElement testWrap = new VisualElement();
            testWrap.name = TEST_INSPECTOR_GUI; 

            if (target is IPublicMethodsInspector publicMethodsTarget)
            {
                VisualElement pubMethodsVisualElement = Public_Methods_Section();
                if (pubMethodsVisualElement.childCount > 1) testWrap.Add(pubMethodsVisualElement);          //Label + Buttons
            }

            if (target is IPrivateMethodInspector privateMethodsTarget)
            {
                VisualElement privateMethodsVisualElement = Private_Method_Section();
                if (privateMethodsVisualElement.childCount > 1) testWrap.Add(privateMethodsVisualElement);  //Label + Buttons
            }

            if (testWrap.childCount > 0)
            { 
                testWrap.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                testWrap.style.marginTop = 10;
                testWrap.style.borderTopWidth = 5;
                testWrap.style.borderTopLeftRadius = 5;
                testWrap.style.borderTopRightRadius = 5;
                testWrap.style.borderBottomRightRadius = 5;
                testWrap.style.borderBottomLeftRadius = 5;
                Label label = new Label("Test Inspector Section");
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.fontSize = 22;
                testWrap.Insert(0, label); 
                 
                root.Add(testWrap); 
            } 
            return root;
        }

        #region DrawStandartEditor
        private void DrawInspectorWithCustomProperties(VisualElement root)
        { 
            SerializedObject serializedObject = new SerializedObject(target); 
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); 
            while (iterator.NextVisible(false))
            { 
                if (IsReactiveProperty(iterator))
                { 
                    root.Add(CreateReactivePropertyField(iterator));
                }
                else if (IsReactiveList(iterator))
                { 
                    root.Add(CreateReactiveListField(iterator));
                }
                else
                { 
                    PropertyField propertyField = new PropertyField(iterator);
                    root.Add(propertyField);
                }
            }
        }
        private bool IsReactiveProperty(SerializedProperty property) => property.displayName.Contains("ReactiveProperty") || property.type.Contains("ReactiveProperty"); 
        private bool IsReactiveList(SerializedProperty property) => property.displayName.Contains("ReactiveList") || property.type.Contains("ReactiveList");
        private VisualElement CreateReactivePropertyField(SerializedProperty property)
        {
            var drawer = new ReactivePropertyPropertyDrawer();
            return drawer.CreatePropertyGUI(property);
        }

        private VisualElement CreateReactiveListField(SerializedProperty property)
        {
            var drawer = new ReactiveListPropertyDrawer();
            return drawer.CreatePropertyGUI(property);
        }

        #endregion

        private bool IsSerializableInUnity(FieldInfo field)
        {
            bool isPublic = field.IsPublic;
            bool hasSerializeField = field.GetCustomAttribute<SerializeField>() != null;
            bool isUnitySerializableType = field.FieldType.IsPrimitive ||
                                           field.FieldType == typeof(string) ||
                                           typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType);

            return (isPublic || hasSerializeField || isUnitySerializableType);
        }

        private VisualElement Private_Method_Section()
        {
            List<VisualElement> content = new List<VisualElement>();

            Type targetType = target.GetType();
            MethodInfo[] publicMethods = targetType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < publicMethods.Length; i++)
            {
                MethodInfo method = publicMethods[i];
                if (method.DeclaringType == typeof(MonoBehaviour) ||
                    method.DeclaringType == typeof(Behaviour) ||
                    method.DeclaringType == typeof(Component) ||
                    method.DeclaringType == typeof(UnityEngine.Object) ||
                    method.DeclaringType == typeof(object)
                    )
                {
                    continue;
                }
                if (method.GetParameters().Length > 0) continue;
                content.Add(new Button(() => { method.Invoke(target, null); }) { text = method.Name });
            }
            return Create_TestSection(content.ToArray());
        } 
        private VisualElement Public_Methods_Section()
        {
            List<VisualElement> content = new List<VisualElement>();

            Type targetType = target.GetType();
            MethodInfo[] publicMethods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance );
            for (int i = 0; i < publicMethods.Length; i++)
            {
                MethodInfo method = publicMethods[i];
                if (method.DeclaringType == typeof(MonoBehaviour) ||
                    method.DeclaringType == typeof(Behaviour) ||
                    method.DeclaringType == typeof(Component) ||
                    method.DeclaringType == typeof(UnityEngine.Object) ||
                    method.DeclaringType == typeof(object)
                    )
                {
                    continue;
                }
                if (method.GetParameters().Length > 0) continue;
                content.Add(new Button(() => { method.Invoke(target, null); }) { text = method.Name }); 
            }
            return Create_TestSection(content.ToArray());
        }

        private VisualElement Create_TestSection(params VisualElement[] content)
        {
            VisualElement root = new VisualElement();
            root.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            root.style.marginTop = 10;

            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(1).GetMethod().Name.ToString().Replace("_", " ");

            Label label = new Label(methodName);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 10;
            root.Add(label);

            foreach (var item in content) root.Add(item);
             
            return root;
        } 
    }


}
