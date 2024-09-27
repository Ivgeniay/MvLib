using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using MvLib.Reactive;

namespace MvLib
{
    [CustomPropertyDrawer(typeof(ReactiveProperty<>), true)]
    internal class ReactivePropertyPropertyDrawer : PropertyDrawer
    { 
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement(); 
            var valueProperty = property.FindPropertyRelative("value");

            if (valueProperty != null)
            { 
                var field = new PropertyField(valueProperty, property.displayName);
                root.Add(field);
            }
            else
            { 
                var label = new Label("No GUI Implemented");
                root.Add(label);
            }

            return root;
        }
    }
}
