using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Lists;
using Aarthificial.Typewriter.Editor.Lists.Items;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Layout {
  public class TableListView : VisualElement {
    private readonly ToolbarSearchField _search;
    public readonly SimpleList List;
    private TypewriterDatabase _database;
    private SerializedObject _so;

    public TableListView() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/TableListView");
      visualTree.CloneTree(this);

      _search = this.Q<ToolbarSearchField>("search");
      List = this.Q<SimpleList>("list");

      _search.RegisterValueChangedCallback(
        evt => List.SearchText = evt.newValue
      );
      List.ExtractValue = property =>
        property.FirstString()?.stringValue ?? string.Empty;
      List.CreateItem = () => new TableListItem();
      List.ExtractValue = property => {
        var table = (DatabaseTable)property.objectReferenceValue;
        return table.TableName;
      };

      var add = this.Q<Button>("add");
      var remove = this.Q<Button>("remove");
      remove.clicked += RemoveTables;

      var creationManipulator = new ContextualMenuManipulator(HandleAddMenu);
      creationManipulator.activators.Clear();
      creationManipulator.activators.Add(
        new ManipulatorActivationFilter { button = MouseButton.LeftMouse }
      );
      add.AddManipulator(creationManipulator);

      RegisterCallback<AttachToPanelEvent>(
        _ => {
          _search.value =
            EditorPrefs.GetString(this.GetViewDataKey() + "_search");
        }
      );

      RegisterCallback<DetachFromPanelEvent>(
        _ => {
          EditorPrefs.SetString(
            this.GetViewDataKey() + "_search",
            _search.value
          );
        }
      );
    }

    private void CreateTable(Type type) {
      if (_database == null) {
        return;
      }

      var path = EditorUtility.SaveFilePanelInProject(
        "Save",
        type.Name,
        "asset",
        "Please enter a file name to save the table to."
      );

      if (string.IsNullOrEmpty(path)) {
        return;
      }

      var index = List.Selection.Count > 0
        ? List.Selection.OrderBy(element => element.Index).First().Index + 1
        : -1;

      Undo.IncrementCurrentGroup();
      var table = (DatabaseTable)ScriptableObject.CreateInstance(type);
      AssetDatabase.CreateAsset(table, path);
      Undo.RegisterCreatedObjectUndo(table, "Create table");
      Undo.RegisterCompleteObjectUndo(table, "Setup table");

      Undo.RecordObject(_database, "Add table");
      index = _database.AddTable(table, index);
      table.TableName = Path.GetFileNameWithoutExtension(path);
      table.Setup(_database);
      EditorUtility.SetDirty(_database);
      Undo.SetCurrentGroupName("Create a new table");

      Refresh();
      List.selectedIndex = index;
    }

    private void RemoveTables() {
      if (_database == null || List.Selection.Count == 0) {
        return;
      }

      var index = int.MaxValue;
      foreach (var data in List.Selection) {
        if (data.Index < index) {
          index = data.Index;
        }

        _database.RemoveTable(_database.Tables[data.Index]);
        AssetDatabase.DeleteAsset(
          AssetDatabase.GetAssetPath(data.Property.objectReferenceValue)
        );
      }

      EditorUtility.SetDirty(_database);
      Refresh();
      if (index > 0) {
        List.selectedIndex = index < _database.Tables.Count ? index : index - 1;
      }
    }

    public void Refresh() {
      if (_so == null) {
        return;
      }

      _so.UpdateIfRequiredOrScript();
      List.Synchronize();
    }

    public void ClearSearch() {
      _search.value = null;
    }

    public void BindDatabase(TypewriterDatabase database) {
      if (_database == database) {
        return;
      }

      _database = database;
      if (database == null) {
        _so = null;
        List.BindProperty(null);
      } else {
        _so = new SerializedObject(_database);
        List.BindProperty(_so.FindProperty(nameof(TypewriterDatabase.Tables)));
      }
    }

    private void HandleAddMenu(ContextualMenuPopulateEvent evt) {
      var mainType = typeof(DatabaseTable);
      var types = TypeCache.GetTypesDerivedFrom(mainType);

      evt.menu.AppendAction(mainType.Name, _ => CreateTable(mainType));
      foreach (var type in types) {
        evt.menu.AppendAction(type.Name, _ => CreateTable(type));
      }
    }

    public new class UxmlFactory : UxmlFactory<TableListView, UxmlTraits> { }
  }
}
