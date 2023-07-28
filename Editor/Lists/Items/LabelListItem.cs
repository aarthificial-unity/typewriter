using Aarthificial.Typewriter.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public class LabelListItem : EditableListItem {
    protected readonly Label Label;
    protected readonly VisualElement Root;
    protected readonly TextField Text;
    protected readonly Label Type;

    public LabelListItem() {
      Root = new VisualElement();
      Root.AddToClassList("editable-item");

      Label = new Label();
      Label.AddToClassList("editable-item__label");
      Text = new TextField { style = { display = DisplayStyle.None } };
      Text.AddToClassList("editable-item__field");

      Type = new Label();
      Type.AddToClassList("editable-item__type");

      Label.RegisterCallback<MouseDownEvent>(HandleMouseDown);
      RegisterCallback<FocusOutEvent>(HandleFocusOut);

      Add(Root);
      Root.Add(Label);
      Root.Add(Text);
      Root.Add(Type);
    }

    protected void SetLabel(string text) {
      Label.text = string.IsNullOrEmpty(text) ? "<empty>" : text;
    }

    protected virtual void HandleFocusOut(FocusOutEvent evt) {
      Label.style.display = DisplayStyle.Flex;
      SetLabel(Text.value);
      Text.style.display = DisplayStyle.None;
    }

    private void HandleMouseDown(MouseDownEvent evt) {
      if (evt.clickCount == 2) {
        Label.style.display = DisplayStyle.None;
        Text.style.display = DisplayStyle.Flex;
        Text.Focus();
      }
    }

    public override void BindProperty(SerializedProperty property) {
      var child = property.FirstString();
      if (child != null) {
        SetLabel(child.stringValue);
        Text.BindProperty(child);
      }
    }

    public override void Unbind() {
      SetLabel("");
      Text.Unbind();
    }
  }
}
