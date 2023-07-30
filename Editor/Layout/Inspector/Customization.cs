using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Lists;
using Aarthificial.Typewriter.Editor.Lists.Items;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout.Inspector {
  public class Customization : VisualElement {
    private readonly TabbedView _container;
    private readonly Dictionary<string, EditableListView> _lists = new();
    private readonly (string, string)[] _possibleProperties = {
      (nameof(RuleEntry.Criteria), "Criteria"),
      (nameof(RuleEntry.Modifications), "Modifications"),
      (nameof(RuleEntry.OnApply), "OnApply"),
      (nameof(RuleEntry.OnInvoke), "OnInvoke"),
    };

    private SerializedProperty _property;

    public Customization() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/Inspector/Customization");
      visualTree.CloneTree(this);

      _container = this.Q<TabbedView>("container");
      _container.AddToClassList("customization");
      _container.TabChanged += index => EnableInClassList("open", index >= 0);
      foreach (var (propertyName, elementName) in _possibleProperties) {
        var list = this.Q<EditableListView>(elementName);
        list.List.CreateItem = CreateListItem;
        list.style.display = DisplayStyle.None;
        _lists.Add(propertyName, list);
      }
    }

    private PropertyListItem CreateListItem() {
      return new PropertyListItem();
    }

    public void BindProperty(SerializedProperty property) {
      if (!property.Update(ref _property)) {
        return;
      }

      foreach (var list in _lists) {
        var listProperty = property?.FindPropertyRelative(list.Key);
        list.Value.BindProperty(listProperty);
        list.Value.RemoveFromHierarchy();
        if (listProperty != null) {
          _container.Add(list.Value);
        }
      }
    }

    public new class UxmlFactory : UxmlFactory<Customization, UxmlTraits> { }
  }
}
