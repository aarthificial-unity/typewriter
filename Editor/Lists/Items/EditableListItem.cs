using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public abstract class EditableListItem : VisualElement {
    public abstract void BindProperty(SerializedProperty property);
    public abstract void Unbind();
  }
}
