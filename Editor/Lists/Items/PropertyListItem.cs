using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public class PropertyListItem : EditableListItem {
    private readonly PropertyField _propertyField;

    public PropertyListItem() {
      _propertyField = new PropertyField();
      Add(_propertyField);

      style.paddingLeft = 8;
      style.paddingRight = 8;
      style.justifyContent = Justify.Center;
    }

    public override void BindProperty(SerializedProperty property) {
      _propertyField.BindProperty(property);
    }

    public override void Unbind() {
      _propertyField.Unbind();
    }
  }
}
