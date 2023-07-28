using Aarthificial.Typewriter.Entries;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public class RuleEntryListItem : BaseEntryListItem, IBindable {
    public IBinding binding { get; set; }
    public string bindingPath { get; set; }

#if UNITY_LOCALIZATION
    private readonly Localization.LocalizedStringBinding _binding;

    public RuleEntryListItem() {
      _binding = new Localization.LocalizedStringBinding(this);
      binding = _binding;
    }

    private void HandleEntryChanged() {
      SetLabel(_binding.Key);
      Text.value = _binding.Key;
    }

    protected override void HandleFocusOut(FocusOutEvent evt) {
      base.HandleFocusOut(evt);
      if (_binding.Key != Text.value) {
        _binding.Key = Text.value;
      }
    }

    public override void BindProperty(SerializedProperty property) {
      var textProperty = property.FindPropertyRelative(
        nameof(LocalizedRuleEntry.Text)
      );
      _binding.Property = textProperty;
      _binding.Changed -= HandleEntryChanged;
      binding = null;

      if (textProperty == null) {
        base.BindProperty(property);
      } else {
        SetLabel(_binding.Key);
        SetType(property);
        SetID(property);
        binding = _binding;
        Text.value = _binding.Key;
        _binding.Changed += HandleEntryChanged;
      }
    }

    public override void Unbind() {
      _binding.Changed -= HandleEntryChanged;
      _binding.Property = null;
      binding = null;

      base.Unbind();
    }
#endif
  }
}
