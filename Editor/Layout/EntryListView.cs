using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Lists;
using Aarthificial.Typewriter.Editor.Lists.Items;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Aarthificial.Typewriter.Editor.Layout {
  public class EntryListView : VisualElement {
    private readonly ScrollView _container;
    private readonly ExpandableListView _events;
    private readonly ExpandableListView _facts;
    private readonly ExpandableListView[] _lists;
    private readonly ExpandableListView _rules;
    private readonly ToolbarSearchField _search;
    private readonly List<SimpleList.ItemData> _selection = new();

    private bool _ignoreSelectionEvents;
    private SerializedObject _so;
    private DatabaseTable _table;

    public EntryListView() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/EntryListView");
      visualTree.CloneTree(this);

      _container = this.Q<ScrollView>("container");
      _facts = this.Q<ExpandableListView>("facts");
      _events = this.Q<ExpandableListView>("events");
      _rules = this.Q<ExpandableListView>("rules");
      _lists = new[] { _events, _facts, _rules };

      _facts.AddManipulator(
        new ContextualMenuManipulator(
          evt => HandleEntryMenu(evt, EntryVariant.Fact)
        )
      );
      _events.AddManipulator(
        new ContextualMenuManipulator(
          evt => HandleEntryMenu(evt, EntryVariant.Event)
        )
      );
      _rules.AddManipulator(
        new ContextualMenuManipulator(
          evt => HandleEntryMenu(evt, EntryVariant.Rule)
        )
      );
      _facts.List.CreateItem = () => new BaseEntryListItem();
      _events.List.CreateItem = () => new BaseEntryListItem();
      _rules.List.CreateItem = () => new RuleEntryListItem();

      foreach (var list in _lists) {
        list.List.RegisterCallback<PointerDownEvent>(
          HandlePointerDown,
          TrickleDown.TrickleDown
        );
        list.List.SelectionChanged += HandleSelection;
        list.List.ExtractValue = ExtractValue;
      }

      _search = this.Q<ToolbarSearchField>("search");
      _search.RegisterValueChangedCallback(HandleSearch);

      var add = this.Q<Button>("add");
      var remove = this.Q<Button>("remove");
      var creationManipulator = new ContextualMenuManipulator(HandleAddMenu);
      creationManipulator.activators.Clear();
      creationManipulator.activators.Add(
        new ManipulatorActivationFilter { button = MouseButton.LeftMouse }
      );
      add.AddManipulator(creationManipulator);
      remove.clicked += HandleRemove;
    }

    public event Action<List<SimpleList.ItemData>> SelectionChanged;

    private string ExtractValue(SerializedProperty property) {
      return property.FindEntry()?.GetKey() ?? string.Empty;
    }

    private void HandleSearch(ChangeEvent<string> evt) {
      foreach (var list in _lists) {
        list.Foldout.value = true;
        list.List.SearchText = evt.newValue;
      }
    }

    private void HandleRemove() {
      RemoveEntries(_lists.SelectMany(list => list.List.Selection));
    }

    private void HandlePointerDown(PointerDownEvent evt) {
      if (evt.button != 1 && (evt.button == 2 || evt.shiftKey || evt.ctrlKey)) {
        return;
      }

      _ignoreSelectionEvents = true;
      foreach (var list in _lists) {
        if (list.List != evt.target) {
          list.List.ClearSelection();
        }
      }

      _ignoreSelectionEvents = false;
    }

    private void HandleSelection(List<SimpleList.ItemData> _) {
      if (_ignoreSelectionEvents) {
        return;
      }

      _selection.Clear();
      foreach (var list in _lists) {
        _selection.AddRange(list.List.Selection);
      }

      SelectionChanged?.Invoke(_selection);
    }

    private void HandleAddMenu(ContextualMenuPopulateEvent evt) {
      var first = true;
      foreach (EntryVariant type in Enum.GetValues(typeof(EntryVariant))) {
        var types = EntryTypeCache.GetTypes(type);
        if (types.Count == 0) {
          continue;
        }

        if (!first) {
          evt.menu.AppendSeparator();
        }

        first = false;
        foreach (var visibleType in types) {
          evt.menu.AppendAction(
            visibleType.Name,
            _ => CreateEntry(visibleType.RealType)
          );
        }
      }
    }

    private void HandleEntryMenu(
      ContextualMenuPopulateEvent evt,
      EntryVariant variant
    ) {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      evt.menu.AppendSeparator();
      var visibleTypes = EntryTypeCache.GetTypes(variant);
      if (visibleTypes.Count > 1) {
        foreach (var visibleType in visibleTypes) {
          evt.menu.AppendAction(
            $"Create/{visibleType.Name}",
            _ => CreateEntry(visibleType.RealType)
          );
        }
      } else if (visibleTypes.Count > 0) {
        evt.menu.AppendAction(
          $"Create {visibleTypes[0].Name}",
          _ => CreateEntry(visibleTypes[0].RealType)
        );
      }

      if (_selection.Count > 0) {
        foreach (var table in TypewriterDatabase.Instance.Tables) {
          evt.menu.AppendAction(
            $"Move To/{table.name}",
            _ => MoveEntries(table)
          );
        }
      }

      var list = evt.target as SimpleList;
      if (list == null && evt.target is ExpandableListView expandableList) {
        list = expandableList.List;
      }

      if (list != null && list.Selection.Count > 0) {
        evt.menu.AppendAction("Remove", _ => RemoveEntries(list.Selection));
      }
    }

    private void CreateEntry(Type entryType) {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      Undo.RecordObject(_table, "Create entry");
      var entry = TypewriterDatabase.Instance.CreateEntry(_table, entryType);
      if (EntryTypeCache.TryGetDescriptor(entry, out var descriptor)) {
        descriptor.HandleEntryCreated(entry, _table);
      }

      Undo.SetCurrentGroupName("Create entry");

      Refresh();
      TypewriterUtils.Events.OnEntrySelected(entry.ID);
    }

    private void MoveEntries(DatabaseTable table) {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      var id = _selection[0].Property.FindEntryID();
      Undo.RecordObjects(new Object[] { _table, table }, "Move to table");
      foreach (var data in _selection) {
        TypewriterDatabase.Instance.MoveToTable(
          data.Property.FindEntryID(),
          table
        );
      }

      Refresh();
      TypewriterUtils.Events.OnEntrySelected(id);
    }

    private void RemoveEntries(IEnumerable<SimpleList.ItemData> entries) {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      Undo.RecordObject(_table, "Remove entries");
      foreach (var itemData in entries) {
        TypewriterDatabase.Instance.RemoveEntry(
          itemData.Property.FindEntryID()
        );
      }

      Refresh();
    }

    public void Refresh() {
      if (_so == null || _table == null) {
        _so = null;
        _table = null;
        return;
      }

      _so.UpdateIfRequiredOrScript();
      _ignoreSelectionEvents = true;
      foreach (var list in _lists) {
        list.List.Synchronize();
      }

      _ignoreSelectionEvents = false;
      HandleSelection(null);
    }

    public void Select(BaseEntry entry) {
      if (_so == null) {
        return;
      }

      _ignoreSelectionEvents = true;
      _search.value = string.Empty;
      foreach (var list in _lists) {
        list.List.ClearSelection();
      }

      IList sourceList = null;
      ExpandableListView parentListView = null;

      switch (entry) {
        case RuleEntry _:
          sourceList = _table.Rules;
          parentListView = _rules;
          break;
        case EventEntry _:
          sourceList = _table.Events;
          parentListView = _events;
          break;
        case FactEntry _:
          sourceList = _table.Facts;
          parentListView = _facts;
          break;
      }

      if (sourceList != null && parentListView != null) {
        var index = sourceList.IndexOf(entry);
        parentListView.List.Select(index);
        if (index > -1
          && index < parentListView.List.contentContainer.childCount) {
          var element = parentListView.List.contentContainer.ElementAt(index);
          if (element != null) {
            _container.ScrollTo(element);
          }
        }
      }

      _ignoreSelectionEvents = false;
      HandleSelection(null);
    }

    public void BindTable(DatabaseTable table) {
      if (_table == table) {
        return;
      }

      _ignoreSelectionEvents = true;
      _table = table;

      if (_table == null) {
        _so = null;
        _facts.BindProperty(null);
        _events.BindProperty(null);
        _rules.BindProperty(null);
      } else {
        _so = new SerializedObject(table);
        _facts.BindProperty(_so.FindProperty(nameof(DatabaseTable.Facts)));
        _events.BindProperty(_so.FindProperty(nameof(DatabaseTable.Events)));
        _rules.BindProperty(_so.FindProperty(nameof(DatabaseTable.Rules)));
      }

      _ignoreSelectionEvents = false;
      HandleSelection(null);
    }

    public new class UxmlFactory : UxmlFactory<EntryListView, UxmlTraits> { }
  }
}
