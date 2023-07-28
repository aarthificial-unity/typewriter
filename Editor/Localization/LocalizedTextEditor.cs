#if UNITY_LOCALIZATION
using Aarthificial.Typewriter.Editor.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Localization {
  public class LocalizedTextEditor : TextField {
    private readonly LocalizedStringBinding _binding;
    private SerializedProperty _property;

    public LocalizedTextEditor(SerializedProperty property) : this() {
      BindProperty(property);
    }

    public LocalizedTextEditor() {
      _binding = new LocalizedStringBinding(this);
      binding = _binding;

      _binding.Changed += HandleEntryChanged;
      this.RegisterValueChangedCallback(HandleFieldChanged);
      Undo.undoRedoPerformed += UndoRedoPerformed;
    }

    ~LocalizedTextEditor() {
      Undo.undoRedoPerformed -= UndoRedoPerformed;
    }

    private void UndoRedoPerformed() {
      SetValueWithoutNotify(_binding.Value);
    }

    private void HandleFieldChanged(ChangeEvent<string> evt) {
      _binding.Value = evt.newValue;
    }

    private void HandleEntryChanged() {
      SetValueWithoutNotify(_binding.Value);
    }

    public void BindProperty(SerializedProperty property) {
      if (!property.Update(ref _property)) {
        return;
      }

      _binding.Property = _property;
    }
  }
}
#endif
