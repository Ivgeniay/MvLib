using UnityEngine.UIElements;

namespace MvLib
{
    [UxmlElement("TestElement")]
    public partial class TestElement : IntegerField
    {
        public TestElement()
        { 
            var label = new Label("Это кастомный элемент Test");
            Add(label); 
        }
    }
}
