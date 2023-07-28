using Aarthificial.Typewriter.Editor.Common;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout.Inspector {
  public class NavigatorButton : VisualElement {
    private readonly Button _button;
    private readonly List<int> _ids = new();
    private readonly List<string> _options = new();

    private int _currentOption = -1;
    private DropdownField _dropdown;
    private string _title;

    public NavigatorButton() {
      _button = new Button();
      _button.clicked += HandleClick;
      Add(_button);
    }

    private string Title {
      get => _title;
      set {
        _title = value;
        _button.text = _title;
        if (_dropdown != null) {
          _dropdown.value = value;
        }
      }
    }

    public void Configure(
      IEnumerable<string> names,
      IEnumerable<int> ids,
      int current = -1
    ) {
      _currentOption = current;
      _options.Clear();
      _options.AddRange(names);
      _ids.Clear();
      _ids.AddRange(ids);

      Refresh();
    }

    private void Refresh() {
      _button.RemoveFromHierarchy();
      if (_dropdown != null) {
        _dropdown.UnregisterValueChangedCallback(HandleChange);
        _dropdown.RemoveFromHierarchy();
        _dropdown = null;
      }

      if (_options.Count > 1) {
        _dropdown = new DropdownField(_options, _currentOption);
        if (_currentOption < 0) {
          _dropdown.value = Title;
        }

        _dropdown.RegisterValueChangedCallback(HandleChange);
        Add(_dropdown);
      } else {
        Add(_button);
        _button.SetEnabled(
          _currentOption < 0 ? _options.Count > 0 : _options.Count > 1
        );
      }
    }

    private void HandleClick() {
      if (_ids.Count < 1) {
        return;
      }

      TypewriterUtils.Events.OnEntrySelected(_ids[0]);
    }

    private void HandleChange(ChangeEvent<string> evt) {
      if (_dropdown.index < 0) {
        return;
      }

      TypewriterUtils.Events.OnEntrySelected(_ids[_dropdown.index]);
    }

    public new class UxmlFactory : UxmlFactory<NavigatorButton, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits {
      private readonly UxmlStringAttributeDescription _title =
        new() { name = "title" };

      public override IEnumerable<UxmlChildElementDescription>
        uxmlChildElementsDescription {
        get {
          yield break;
        }
      }

      public override void Init(
        VisualElement ve,
        IUxmlAttributes bag,
        CreationContext cc
      ) {
        base.Init(ve, bag, cc);
        ((NavigatorButton)ve).Title = _title.GetValueFromBag(bag, cc);
      }
    }
  }
}
