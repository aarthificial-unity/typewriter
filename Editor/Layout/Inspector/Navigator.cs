using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout.Inspector {
  public class Navigator : VisualElement {
    private readonly NavigatorButton _alternative;
    private readonly List<int> _ids = new();
    private readonly List<string> _names = new();
    private readonly NavigatorButton _next;
    private readonly NavigatorButton _previous;

    private BaseEntry _entry;
    private SerializedProperty _property;

    public Navigator() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/Inspector/Navigator");
      visualTree.CloneTree(this);

      _previous = this.Q<NavigatorButton>("previous");
      _alternative = this.Q<NavigatorButton>("alternative");
      _next = this.Q<NavigatorButton>("next");

      TypewriterUtils.Events.LookupCreated += Refresh;
    }

    ~Navigator() {
      TypewriterUtils.Events.LookupCreated -= Refresh;
    }

    public void BindProperty(SerializedProperty property) {
      if (!property.Update(ref _property)) {
        return;
      }

      _entry = _property?.FindEntry();
      Refresh();
    }

    private void Refresh() {
      EntryTypeCache.TryGetDescriptor(_entry, out var descriptor);
      var current = -1;
      _names.Clear();
      _ids.Clear();
      descriptor?.CreateNextMenu(_entry, _names, _ids, ref current);
      _next.Configure(_names, _ids, current);

      current = -1;
      _names.Clear();
      _ids.Clear();
      descriptor?.CreateAlternativeMenu(_entry, _names, _ids, ref current);
      _alternative.Configure(_names, _ids, current);

      current = -1;
      _names.Clear();
      _ids.Clear();
      descriptor?.CreatePreviousMenu(_entry, _names, _ids, ref current);
      _previous.Configure(_names, _ids, current);
    }

    public new class UxmlFactory : UxmlFactory<Navigator, UxmlTraits> { }
  }
}
