using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.References {
  public class DatabaseTreePopup : PopupWindowContent {
    private static readonly GUIStyle _buttonToggleStyle = new("Button") {
      padding = new RectOffset(0, 0, 0, 0),
    };
    private readonly List<(EntryVariant, GUIContent)> _availableTypes;

    private readonly SearchField _searchField;
    private readonly DatabaseTreeView _treeView;
    private EntryVariant _filterVariant;
    private bool _shouldClose;

    public DatabaseTreePopup(DatabaseTreeView contents) {
      var filter = contents.Filter;
      _filterVariant = filter.Variant & filter.PreferredVariant;

      if (filter.BaseType != null
        && EntryTypeCache.TryGetDescriptor(
          filter.BaseType,
          out var descriptor
        )) {
        _availableTypes = new List<(EntryVariant, GUIContent)> {
          (descriptor.Variant,
            new GUIContent { image = descriptor.Variant.GetIcon() }),
        };
      } else {
        _availableTypes = filter.Variant.GetMatching()
          .Select(type => (type, new GUIContent { image = type.GetIcon() }))
          .ToList();
      }

      _searchField = new SearchField();
      _treeView = contents;
    }

    public float Width { get; set; }

    public override void OnGUI(Rect rect) {
      const int border = 4;
      const int topPadding = 12;
      const int searchHeight = 16;
      const int remainTop = topPadding + searchHeight + border;

      var toggleWidth = _availableTypes.Count > 1
        ? _availableTypes.Count * 24 + border
        : 0;

      var searchRect = new Rect(
        border,
        topPadding,
        rect.width - toggleWidth - border * 2,
        searchHeight
      );
      var toggleRect = new Rect(
        searchRect.width + border * 2,
        topPadding,
        toggleWidth - border,
        searchHeight
      );
      var remainingRect = new Rect(
        border,
        topPadding + searchHeight + border,
        rect.width - border * 2,
        rect.height - remainTop - border
      );

      EditorGUI.BeginChangeCheck();
      toggleRect.width /= _availableTypes.Count;
      foreach (var (entryType, guiContent) in _availableTypes) {
        if (GUI.Toggle(
            toggleRect,
            _filterVariant.HasFlag(entryType),
            guiContent,
            _buttonToggleStyle
          )) {
          _filterVariant |= entryType;
        } else {
          _filterVariant &= ~entryType;
        }

        toggleRect.x += toggleRect.width;
      }

      if (EditorGUI.EndChangeCheck()) {
        _treeView.FilterVariant = _filterVariant;
      }

      _treeView.searchString = _searchField.OnGUI(
        searchRect,
        _treeView.searchString
      );
      _treeView.OnGUI(remainingRect);

      if (_shouldClose) {
        GUIUtility.hotControl = 0;
        editorWindow.Close();
      }

      if (_treeView.HasSelection()) {
        ForceClose();
      }
    }

    public override void OnOpen() {
      _searchField.SetFocus();
      base.OnOpen();
    }

    public override Vector2 GetWindowSize() {
      var result = base.GetWindowSize();
      result.x = Width;
      return result;
    }

    private void ForceClose() {
      _shouldClose = true;
    }
  }
}
