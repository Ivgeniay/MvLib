using UnityEditor.UIElements;
using UnityEngine.UIElements;
using MvLib.Reactive; 
using UnityEditor;

namespace MvLib
{
    [CustomPropertyDrawer(typeof(ReactiveList<>), true)]
    internal class ReactiveListPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var valueProperty = property.FindPropertyRelative("list");
            var field = new PropertyField(valueProperty, property.displayName);
            root.Add(field);
            return root;
        }
    }
}
