using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Lists;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout {
  /// <summary>
  ///   Displays the contents of a Typewriter database using a two-column
  ///   layout.
  /// </summary>
  public class DatabaseView : VisualElement {
    private readonly VisualElement _content;
    private readonly Button _create;
    private readonly VisualElement _empty;
    private readonly EntryListView _entries;
    private readonly TableListView _tables;

    private TypewriterDatabase _database;
    private bool _isInitialized;

    public DatabaseView() {
      TypewriterUtils.Events.EntrySelected += HandleEntrySelected;
      TypewriterUtils.Events.TableSelected += HandleTableSelected;
      TypewriterUtils.Events.DatabaseRemoved += HandleDeleteDatabase;
      TypewriterUtils.Events.DatabaseCreated += HandleCreateDatabase;

      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/DatabaseView");
      visualTree.CloneTree(this);

      _content = this.Q<VisualElement>("content");
      _empty = this.Q<VisualElement>("empty");
      _create = this.Q<Button>("create");
      _tables = this.Q<TableListView>("tables");
      _entries = this.Q<EntryListView>("entries");

      _tables.List.SelectionChanged += HandleTableChanged;
      _entries.SelectionChanged += HandleEntryChanged;

      Undo.undoRedoPerformed += UndoRedoPerformed;
      _create.clicked += TypewriterUtils.CreateDatabase;

      if (TypewriterDatabase.Instance == null) {
        HandleDeleteDatabase();
      } else {
        HandleCreateDatabase();
      }
    }

    public SerializedProperty TableProperty { get; private set; }
    public SerializedProperty EntryProperty { get; private set; }
    public event Action SelectionChanged;

    ~DatabaseView() {
      TypewriterUtils.Events.DatabaseRemoved -= HandleDeleteDatabase;
      TypewriterUtils.Events.DatabaseCreated -= HandleCreateDatabase;
      TypewriterUtils.Events.EntrySelected -= HandleEntrySelected;
      TypewriterUtils.Events.TableSelected -= HandleTableSelected;
      _create.clicked -= TypewriterUtils.CreateDatabase;
      Undo.undoRedoPerformed -= UndoRedoPerformed;
    }

    private void HandleCreateDatabase() {
      _database = TypewriterDatabase.Instance;
      _tables.BindDatabase(_database);
      _content.style.display = DisplayStyle.Flex;
      _empty.style.display = DisplayStyle.None;
      _isInitialized = true;
    }

    private void HandleDeleteDatabase() {
      _database = null;
      _tables.BindDatabase(null);
      _content.style.display = DisplayStyle.None;
      _empty.style.display = DisplayStyle.Flex;
      _isInitialized = false;
    }

    private void UndoRedoPerformed() {
      if (!_isInitialized) {
        return;
      }

      _tables.Refresh();
      _entries.Refresh();
    }

    private void HandleTableSelected(DatabaseTable table) {
      if (!_isInitialized) {
        return;
      }

      _tables.ClearSearch();
      _tables.List.Select(_database.Tables.IndexOf(table));
    }

    private void HandleEntrySelected(BaseEntry entry) {
      if (!_isInitialized) {
        return;
      }

      if (!_database.TryGetTable(entry.ID, out var table)) {
        return;
      }

      _tables.ClearSearch();
      _tables.List.Select(_database.Tables.IndexOf(table));
      _entries.Select(entry);
    }

    private void HandleEntryChanged(List<SimpleList.ItemData> selection) {
      EntryProperty = selection.Count > 0 ? selection[0].Property : null;
      SelectionChanged?.Invoke();
    }

    private void HandleTableChanged(List<SimpleList.ItemData> selection) {
      if (selection.Count > 0 && _database.Tables.Count > selection[0].Index) {
        TableProperty = selection[0].Property;
        _entries.BindTable(_database.Tables[selection[0].Index]);
      } else {
        TableProperty = null;
        _entries.BindTable(null);
      }
    }

    public new class UxmlFactory : UxmlFactory<DatabaseView, UxmlTraits> { }
  }
}
