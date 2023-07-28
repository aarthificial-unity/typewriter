using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists {
  public class EditableListView : BindableElement {
    public readonly SimpleList List;

    private SerializedProperty _property;

    public Action<int> CreateElement;
    public Action<IEnumerable<SimpleList.ItemData>> RemoveElements;

    private readonly Label _title;

    public EditableListView() : this("UXML/Lists/EditableListView") { }

    protected EditableListView(string templatePath) {
      var visualTree = Resources.Load<VisualTreeAsset>(templatePath);
      visualTree.CloneTree(this);

      List = this.Q<SimpleList>("list");
      _title = this.Q<Label>("title");
      var add = this.Q<Button>("add");
      var remove = this.Q<Button>("remove");

      add.clicked += HandleAdd;
      remove.clicked += HandleRemove;
      List.AddManipulator(new ContextualMenuManipulator(HandleContextMenu));
    }

    public bool Reorderable {
      get => List.reorderable;
      set => List.reorderable = value;
    }

    public SelectionType SelectionType {
      get => List.selectionType;
      set => List.selectionType = value;
    }

    public string Title {
      get => _title.tooltip;
      set => _title.text = value;
    }

    public string TitleTooltip {
      get => _title.tooltip;
      set => _title.tooltip = value;
    }

    private void HandleContextMenu(ContextualMenuPopulateEvent evt) {
      evt.menu.AppendAction("Add", _ => HandleAdd());
      if (List.Selection.Count > 0) {
        evt.menu.AppendAction("Remove", _ => HandleRemove());
      }
    }

    public void BindProperty(SerializedProperty property) {
      _property = property;
      List.BindProperty(property);
    }

    protected virtual void HandleAdd() {
      if (_property?.serializedObject.targetObject == null) {
        return;
      }

      var index = List.Selection.Count > 0
        ? List.Selection.Last().Index + 1
        : _property.arraySize;

      if (CreateElement == null) {
        _property.InsertArrayElementAtIndex(index);
        _property.serializedObject.ApplyModifiedProperties();
      } else {
        CreateElement(index);
        _property.serializedObject.UpdateIfRequiredOrScript();
      }

      List.Synchronize();
      if (index > -1 && index < List.Source.Count) {
        List.selectedIndex = index;
      }
    }

    private void HandleRemove() {
      if (_property == null) {
        return;
      }

      var indices = new List<int>(List.selectedIndices);
      if (indices.Count == 0) {
        return;
      }

      List.ClearSelection();

      indices.Sort();
      var firstIndex = indices[0];
      indices.Reverse();

      if (RemoveElements == null) {
        foreach (var index in indices) {
          _property.DeleteArrayElementAtIndex(List.Source[index].Index);
        }

        _property.serializedObject.ApplyModifiedProperties();
      } else {
        RemoveElements(indices.Select(index => List.Source[index]));
        _property.serializedObject.UpdateIfRequiredOrScript();
      }

      List.Synchronize();
      if (firstIndex > 0) {
        List.selectedIndex = firstIndex - 1;
      }
    }

    public new class UxmlFactory : UxmlFactory<EditableListView, UxmlTraits> { }

    public new class UxmlTraits : BindableElement.UxmlTraits {
      private readonly UxmlBoolAttributeDescription _reorderable =
        new() { name = "reorderable" };
      private readonly UxmlStringAttributeDescription _title =
        new() { name = "title" };
      private readonly UxmlStringAttributeDescription _tooltip =
        new() { name = "title-tooltip" };

      private readonly UxmlEnumAttributeDescription<SelectionType>
        _selectionType = new() { name = "selection-type" };

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
        var list = (EditableListView)ve;
        list.Reorderable = _reorderable.GetValueFromBag(bag, cc);
        list.SelectionType = _selectionType.GetValueFromBag(bag, cc);
        list.Title = _title.GetValueFromBag(bag, cc);
        list.TitleTooltip = _tooltip.GetValueFromBag(bag, cc);
      }
    }
  }
}
