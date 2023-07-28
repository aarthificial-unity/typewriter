using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Lists.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists {
  public class SimpleList : ListView {
    private readonly ScrollView _scrollView;
    public readonly List<ItemData> Selection = new();
    public readonly List<ItemData> Source = new();
    private List<int> _initialSelection;
    private SerializedProperty _property;
    private string _searchText;

    public Func<EditableListItem> CreateItem = () => new LabelListItem();
    public Func<SerializedProperty, string> ExtractValue = property =>
      property.FirstString()?.stringValue ?? string.Empty;

    public SimpleList() {
      makeItem = MakeItem;
      bindItem = BindItem;
      unbindItem = UnbindItem;
      itemsSource = Source;
      onSelectionChange += _ => UpdateSelection();
      itemIndexChanged += HandleItemIndexChanged;
      _scrollView = this.Q<ScrollView>();

      this.AddManipulator(new ContextualMenuManipulator(HandleContextMenu));

      this.Q<ScrollView>()
        .RegisterCallback<PointerDownEvent>(
          evt => {
            if (evt.button != 1) {
              return;
            }

            var localPosition = evt.localPosition;
            var num = (int)(localPosition.y / (double)fixedItemHeight);
            if (num < Source.Count && !selectedIndices.Contains(num)) {
              SetSelection(num);
            }
          }
        );

      RegisterCallback<AttachToPanelEvent>(
        _ => {
          _initialSelection = EditorPrefs.GetString(this.GetViewDataKey())
            .Split(',')
            .Where(value => int.TryParse(value, out var _))
            .Select(int.Parse)
            .ToList();

          if (_property != null) {
            SelectInitial();
          }
        }
      );

      RegisterCallback<DetachFromPanelEvent>(
        _ => {
          EditorPrefs.SetString(
            this.GetViewDataKey(),
            string.Join(",", selectedIndices)
          );
        }
      );
    }

    public string SearchText {
      get => _searchText;
      set {
        if (_searchText == value) {
          return;
        }

        _searchText = value;
        Synchronize();
      }
    }

    public override VisualElement contentContainer =>
      _scrollView.contentContainer;

    public event Action Synchronized;
    public event Action<List<ItemData>> SelectionChanged;

    private void HandleItemIndexChanged(int from, int to) {
      _property.MoveArrayElement(from, to);
      _property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
      if (selectedIndices.Contains(from)) {
        SetSelectionWithoutNotify(
          selectedIndices.Where(id => id != from).Append(to).ToList()
        );
      }

      Synchronize();
    }

    private void HandleContextMenu(ContextualMenuPopulateEvent evt) {
      evt.menu.AppendAction("Refresh", _ => { Synchronize(); });
    }

    private void UnbindItem(VisualElement item, int index) {
      if (item is EditableListItem editableListItem) {
        editableListItem.Unbind();
      }
    }

    private void BindItem(VisualElement item, int id) {
      if (item is EditableListItem editableListItem
        && id >= 0
        && Source.Count > id) {
        editableListItem.BindProperty(Source[id].Property);
      }
    }

    private VisualElement MakeItem() {
      return CreateItem();
    }

    private void SelectInitial() {
      if (_initialSelection == null) {
        return;
      }

      SetSelection(
        _initialSelection.Where(index => index < Source.Count && index >= 0)
      );
      _initialSelection = null;
    }

    public void Synchronize() {
      Source.Clear();

      if (_property != null) {
        FillItems();
      }

      if (style.flexGrow.value == 0) {
        style.height = Source.Count * fixedItemHeight;
      }

      Rebuild();
      SelectInitial();
      UpdateSelection();
      Synchronized?.Invoke();
    }

    private void FillItems() {
      var isEmpty = string.IsNullOrEmpty(_searchText) || ExtractValue == null;

      for (var i = 0; i < _property.arraySize; i++) {
        var property = _property.GetArrayElementAtIndex(i);
        if (!isEmpty) {
          var value = ExtractValue(property);
          if (value.IndexOf(
              _searchText,
              StringComparison.OrdinalIgnoreCase
            )
            < 0) {
            continue;
          }
        }

        Source.Add(
          new ItemData {
            Property = property,
            Index = i,
          }
        );
      }
    }

    public void BindProperty(SerializedProperty property) {
      if (property.Update(ref _property)) {
        Synchronize();
      }
    }

    public void Select(int index) {
      if (index >= 0 && index < Source.Count) {
        selectedIndex = index;
      }
    }

    private void UpdateSelection() {
      Selection.Clear();
      foreach (var index in selectedIndices) {
        if (index >= 0 && Source.Count > index) {
          Selection.Add(Source[index]);
        }
      }

      SelectionChanged?.Invoke(Selection);
    }

    public class ItemData {
      public int Index;
      public SerializedProperty Property;
    }

    public new class UxmlFactory : UxmlFactory<SimpleList, UxmlTraits> { }
  }
}
