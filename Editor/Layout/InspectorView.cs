using Aarthificial.Typewriter.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout {
  public class InspectorView : VisualElement {
    private readonly PropertyField _field;
    private SerializedProperty _property;

    public InspectorView() {
      _field = new PropertyField { style = { flexGrow = 1 } };
    }

    public void BindProperty(SerializedProperty property) {
      if (!property.Update(ref _property)) {
        return;
      }

      if (_property == null) {
        _field.Unbind();
        _field.RemoveFromHierarchy();
      } else {
        _field.BindProperty(_property);
        if (_field.hierarchy.parent == null) {
          Add(_field);
        }
      }
    }

    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
  }
}
