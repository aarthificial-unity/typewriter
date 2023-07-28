using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Entries;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout.Inspector {
  public class Toolbar : BindableElement {
#if UNITY_LOCALIZATION
    private readonly Localization.LocalizedStringBinding _binding;
#endif
    private readonly Label _titleCategory;
    private readonly Label _titleEntry;
    private readonly Label _titleEntryId;
    private SerializedProperty _property;
    private SerializedObject _tableSo;

    public Toolbar() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/Inspector/Toolbar");
      visualTree.CloneTree(this);

      _titleCategory = this.Q<Label>("titleCategory");
      _titleEntry = this.Q<Label>("titleEntry");
      _titleEntryId = this.Q<Label>("titleEntryId");
#if UNITY_LOCALIZATION
      _binding = new Localization.LocalizedStringBinding(this);
      binding = _binding;
#endif
    }

    public void BindCategory(SerializedProperty property) {
      var table = property?.objectReferenceValue as DatabaseTable;
      if (_tableSo?.targetObject == table) {
        return;
      }

      if (table == null) {
        _tableSo = null;
        _titleCategory.Unbind();
        _titleCategory.text = "";
      } else {
        _tableSo = new SerializedObject(table);
        _titleCategory.BindProperty(_tableSo.FindProperty("m_Name"));
      }
    }

    public void BindEntry(SerializedProperty property) {
      if (!property.Update(ref _property)) {
        return;
      }

      _titleEntry.Unbind();
#if UNITY_LOCALIZATION
      _binding.Changed -= HandleEntryChanged;
      _binding.Property = null;
#endif

      if (property?.FindPropertyRelative(nameof(BaseEntry.ID)) == null) {
        _titleEntry.text = "";
        _titleEntryId.text = "";
      } else {
        _titleEntryId.text = property.FindPropertyRelative(nameof(BaseEntry.ID))
          .intValue.ToString();

#if UNITY_LOCALIZATION
        var textProperty =
          property.FindPropertyRelative(nameof(LocalizedRuleEntry.Text));
        if (textProperty == null) {
          _titleEntry.BindProperty(
            property.FindPropertyRelative(nameof(BaseEntry.Key))
          );
        } else {
          _binding.Property = textProperty;
          _binding.Changed += HandleEntryChanged;
          HandleEntryChanged();
        }
#else
        _titleEntry.BindProperty(
          property.FindPropertyRelative(nameof(BaseEntry.Key))
        );
#endif
      }
    }

#if UNITY_LOCALIZATION
    private void HandleEntryChanged() {
      _titleEntry.text = _binding.Key;
    }
#endif

    public new class UxmlFactory : UxmlFactory<Toolbar, UxmlTraits> { }
  }
}
