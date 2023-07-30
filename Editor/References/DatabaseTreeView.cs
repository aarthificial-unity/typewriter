using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.References {
  /// <summary>
  ///   Displays the contents of a Typewriter database in a form of expandable
  ///   tree.
  /// </summary>
  public class DatabaseTreeView : TreeView {
    private static readonly Texture2D _tableIcon =
      Resources.Load<Texture2D>("Textures/Table");

    private readonly BaseEntry _currentEntry;
    private readonly Action<BaseEntry> _selectionHandler;
    public readonly EntryFilterAttribute Filter;
    private EntryVariant _filterVariant;
    private int _selectedId = -1;

    public DatabaseTreeView(
      BaseEntry currentEntry,
      Action<BaseEntry> selectionHandler,
      EntryFilterAttribute filter
    ) : base(new TreeViewState()) {
      Filter = filter;
      _currentEntry = currentEntry;
      _selectionHandler = selectionHandler;
      _filterVariant = Filter.Variant & Filter.PreferredVariant;
      showAlternatingRowBackgrounds = true;
      showBorder = true;
      Reload();
    }

    public EntryVariant FilterVariant {
      set {
        if (_filterVariant == value) {
          return;
        }

        _filterVariant = value;
        Reload();
      }
    }
    private TreeViewItem Root { get; set; }

    protected override bool CanMultiSelect(TreeViewItem item) {
      return false;
    }

    protected override TreeViewItem BuildRoot() {
      Root = new TreeViewItem(-1, -1);
      var id = 1;
      var emptyChild = new CollectionTreeViewItem(null, id++) {
        displayName = $"None ({Filter.GetName()})",
      };

      if (Filter.AllowEmpty) {
        Root.AddChild(emptyChild);
      }

      var tables = TypewriterDatabase.Instance.Tables;
      if (Filter.TableName != null) {
        tables = tables.Where(table => table.TableName == Filter.TableName)
          .ToList();
      }

      var groups = new List<TreeViewItem>();
      BaseEntry firstEntry = null;
      foreach (var table in tables) {
        var types = _filterVariant.GetMatching().ToList();
        var entryLists = Filter.BaseType == null
          ? types.Select(type => table.GetEntriesOfType(type).ToList()).ToList()
          : types.Select(
              type => table.GetEntriesOfType(type)
                .Where(entry => Filter.BaseType.IsInstanceOfType(entry))
                .ToList()
            )
            .ToList();

        var nonEmpty = types.Where((type, index) => entryLists[index].Count > 0)
          .ToList();
        if (nonEmpty.Count == 0) {
          continue;
        }

        var isSingleType = nonEmpty.Count == 1;
        var group = new TreeViewItem(id++) {
          displayName =
            isSingleType ? $"{table.name}/{nonEmpty[0]}" : table.name,
          icon = _tableIcon,
        };

        for (var index = 0; index < types.Count; index++) {
          var type = types[index];
          var entries = entryLists[index];

          if (entries.Count == 0) {
            continue;
          }

          firstEntry ??= entries[0];

          var subGroup = isSingleType
            ? group
            : new TreeViewItem(id++) { displayName = type.ToString() };

          foreach (var entry in entries) {
            if (entry == _currentEntry) {
              _selectedId = id;
            }

            var item =
              new CollectionTreeViewItem(entry, id++) {
                displayName = entry.GetKey(),
              };
            subGroup.AddChild(item);
          }

          if (!isSingleType) {
            group.AddChild(subGroup);
          }
        }

        groups.Add(group);
      }

      if (groups.Count == 1) {
        foreach (var child in groups[0].children) {
          Root.AddChild(child);
        }
      } else {
        foreach (var group in groups) {
          Root.AddChild(group);
        }
      }

      if (!Filter.AllowEmpty && firstEntry == null) {
        Root.AddChild(emptyChild);
      }

      SetupDepthsFromParentsAndChildren(Root);

      return Root;
    }

    public override void OnGUI(Rect rect) {
      if (_selectedId > -1) {
        FrameItem(_selectedId);
        _selectedId = -1;
      }

      base.OnGUI(rect);
    }

    protected override void SelectionChanged(IList<int> selectedIds) {
      if (FindItem(selectedIds[0], rootItem) is CollectionTreeViewItem item) {
        _selectionHandler(item.Entry);
      } else {
        SetExpanded(selectedIds[0], !IsExpanded(selectedIds[0]));
        SetSelection(new int[] { });
      }
    }

    private class CollectionTreeViewItem : TreeViewItem {
      private readonly EntryVariant _variant;
      public readonly BaseEntry Entry;

      public CollectionTreeViewItem(BaseEntry entry, int id) : base(id, 0) {
        Entry = entry;
        if (entry != null
          && EntryTypeCache.TryGetDescriptor(
            entry.GetType(),
            out var descriptor
          )) {
          _variant = descriptor.Variant;
        }
      }

      public override Texture2D icon {
        get => Entry == null ? null : _variant.GetIcon();
        set { }
      }
    }
  }
}
