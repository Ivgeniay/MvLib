using UnityEngine.UIElements;
using UnityEditor.UIElements;
using MvLib.Collections;
using UnityEditor;
using UnityEngine;
using System; 

namespace MvLib
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    internal class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        { 
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            VisualElement headerVisualElementWrap = new VisualElement();

            CreateHeader(property, headerVisualElementWrap);

            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");

            if (keysProperty == null || valuesProperty == null)
            {
                root.Add(new Label("Invalid SerializableDictionary"));
                return root;
            }

            VisualElement keyValueVisualElementWrap = new VisualElement();
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                AddDictionaryElement(keyValueVisualElementWrap, keysProperty, valuesProperty, i, property);
            }
            Button addButton = new Button(() =>
            {
                keysProperty.arraySize += 1;
                valuesProperty.arraySize += 1;
                property.serializedObject.ApplyModifiedProperties(); 
                AddDictionaryElement(
                    keyValueVisualElementWrap, 
                    keysProperty, 
                    valuesProperty,
                    keysProperty.arraySize == 0 ? 0 : keysProperty.arraySize - 1, 
                    property);
                property.serializedObject.Update();
            })
            {
                text = "Add"
            };

            root.Add(headerVisualElementWrap);
            root.Add(keyValueVisualElementWrap);
            root.Add(addButton);

            return root;
        }

        private void CreateHeader(SerializedProperty property, VisualElement container)
        {
            var header = new VisualElement();

            VisualElement headerLableWrap = new VisualElement();
            headerLableWrap.style.flexDirection = FlexDirection.Row;
            Label headerL = new Label($" (SerializableDictionary)");
            Label diasplayNameLabel = new Label($"{property.displayName}");
            diasplayNameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            headerLableWrap.Add(diasplayNameLabel);
            headerLableWrap.Add(headerL);
            headerLableWrap.style.paddingBottom = 8;
            headerLableWrap.style.paddingTop = 8;

            VisualElement keyLabelWrap = new VisualElement();
            keyLabelWrap.style.flexDirection = FlexDirection.Row;

            var keyLabel = new Label("Key");
            keyLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            keyLabel.style.flexGrow = 1;

            var valueLabel = new Label("Value");
            valueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            valueLabel.style.flexGrow = 1;

            keyLabelWrap.Add(keyLabel);
            keyLabelWrap.Add(valueLabel);

            header.Add(headerLableWrap);
            header.Add(keyLabelWrap);

            container.Add(header);
        }
        private void AddDictionaryElement(VisualElement container, SerializedProperty keysProperty, SerializedProperty valuesProperty, int index, SerializedProperty property)
        {
            SerializedProperty keyProperty = keysProperty.GetArrayElementAtIndex(index);
            SerializedProperty valueProperty = valuesProperty.GetArrayElementAtIndex(index);

            DictionaryElement dictionaryElement = new DictionaryElement(keyProperty, valueProperty);
            dictionaryElement.SetCallback(() =>
            {
                RemoveItem(container, keysProperty, valuesProperty, index, property, dictionaryElement);
            });
            container.Add(dictionaryElement); 
        }
        private void RemoveItem(VisualElement container, SerializedProperty keysProperty, SerializedProperty valuesProperty, int index, SerializedProperty property, DictionaryElement dictionaryElement)
        {
            if (index < keysProperty.arraySize && index < valuesProperty.arraySize)
            {
                keysProperty.DeleteArrayElementAtIndex(index);
                valuesProperty.DeleteArrayElementAtIndex(index);

                container.Remove(dictionaryElement);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }
        }
    }


    public class DictionaryElement : VisualElement
    {
        private Button removeButton;
        public DictionaryElement(SerializedProperty keyProperty, SerializedProperty valueProperty, Action onRemove = null)
        {
            style.flexDirection = FlexDirection.Row;

            PropertyField keyField = new PropertyField(keyProperty); 
            keyField.style.width = 300;
            keyField.style.maxWidth = 300;
            Add(keyField);

            PropertyField valueField = new PropertyField(valueProperty);
            valueField.style.width = 100;
            valueField.style.maxWidth = 100;
            Add(valueField);
             
            removeButton = new Button(onRemove)
            {
                text = "Remove"
            };
            removeButton.style.flexGrow = 0;
            Add(removeButton);
        }

        public void SetRemoveButtonEnabled(bool enabled)
        {
            removeButton.SetEnabled(enabled);
        }

        public void SetCallback(Action callback)
        {
            removeButton.clicked += callback;
        }
    }
}

