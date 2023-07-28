using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.References {
  [CustomPropertyDrawer(typeof(EntryReference))]
  public class EntryReferencePropertyDrawer : PropertyDrawer {
    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var idProperty = property.FindPropertyRelative(
        nameof(EntryReference.InternalID)
      );
      var filter = GetFilter();
      var shouldRecreateLookup = ShouldRecreateLookup();

      EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, label);
      CollectionEntryField(
        position,
        idProperty,
        position,
        filter,
        shouldRecreateLookup
      );
      EditorGUI.EndProperty();
    }

    protected virtual EntryFilterAttribute GetFilter() {
      var attributes = fieldInfo.GetCustomAttributes(
        typeof(EntryFilterAttribute),
        false
      );
      return attributes.Length > 0
        ? (EntryFilterAttribute)attributes[0]
        : new EntryFilterAttribute();
    }

    protected bool ShouldRecreateLookup() {
      var attributes = fieldInfo.GetCustomAttributes(
        typeof(RecreateLookupAttribute),
        false
      );
      return attributes.Length > 0;
    }

    public static void CollectionEntryField(
      Rect position,
      SerializedProperty property,
      Rect popupPosition,
      EntryFilterAttribute filter,
      bool recreateLookup = false
    ) {
      property.intValue = CollectionEntryPicker(
        position,
        property.intValue,
        id => {
          property.intValue = id;
          property.serializedObject.ApplyModifiedProperties();
          if (recreateLookup) {
            TypewriterUtils.RecreateLookup();
          }
        },
        popupPosition,
        filter
      );
    }

    public static int CollectionEntryPicker(
      Rect position,
      int previousId,
      Action<int> selectionHandler,
      Rect popupPosition,
      EntryFilterAttribute filter
    ) {
      TypewriterDatabase.Instance.TryGetEntry(previousId, out var entry);
      TypewriterDatabase.Instance.TryGetTable(previousId, out var table);

      if (DropEntry(position, filter, out var dropEntry)) {
        previousId = dropEntry.ID;
      }

      if (previousId == 0 && !filter.AllowEmpty) {
        entry = filter.FindFirstMatch();
        if (entry != null) {
          previousId = entry.ID;
          TypewriterDatabase.Instance.TryGetTable(entry.ID, out table);
        }
      }

      if (DrawShowEntryButton(ref position, entry)
        && TypewriterEditor.Instance != null) {
        var selected = TypewriterEditor.Instance.GetSelectedEntry();
        if (selected != 0
          && TypewriterDatabase.Instance.TryGetEntry(
            selected,
            out var selectedEntry
          )
          && filter.Test(selectedEntry)) {
          previousId = selected;
          entry = selectedEntry;
          TypewriterDatabase.Instance.TryGetTable(selected, out table);
        }
      }

      var label = new GUIContent();
      if (previousId == 0) {
        label.text = $"None ({filter.GetName()})";
      } else if (entry == null || table == null || !filter.Test(entry)) {
        label.text = $"Missing {previousId} ({filter.GetName()})";
      } else {
        var displayName = entry.GetKey();
        label.text = $"{table.name}/{displayName}";
        var size = EditorStyles.miniPullDown.CalcSize(label);

        if (EntryTypeCache.TryGetDescriptor(
            entry.GetType(),
            out var descriptor
          )) {
          label.image = descriptor.Type.GetIcon();
          filter = filter.Copy();
          filter.PreferredType |= descriptor.Type;
        }

        if (position.width < size.x) {
          label.tooltip = label.text;
          label.text = displayName;
        }
      }

      if (EditorGUI.DropdownButton(position, label, FocusType.Passive)) {
        PopupWindow.Show(
          popupPosition,
          new DatabaseTreePopup(
            new DatabaseTreeView(
              entry,
              selection => selectionHandler(selection?.ID ?? 0),
              filter
            )
          ) { Width = Mathf.Max(popupPosition.width, 300) }
        );
      }

      return previousId;
    }

    public static bool DropEntry(
      Rect position,
      EntryFilterAttribute filter,
      out BaseEntry entry
    ) {
      if (Event.current.type == EventType.DragUpdated
        || Event.current.type == EventType.DragPerform) {
        if (DragAndDrop.paths.Length > 0
          && position.Contains(Event.current.mousePosition)
          && int.TryParse(DragAndDrop.paths[0], out var id)
          && TypewriterDatabase.Instance.TryGetEntry(id, out entry)
          && filter.Test(entry)) {
          DragAndDrop.visualMode = DragAndDropVisualMode.Link;
          return Event.current.type != EventType.DragUpdated;
        }
      }

      entry = null;
      return false;
    }

    public static bool DrawShowEntryButton(ref Rect position, BaseEntry entry) {
      var fetch = false;
      var width = position.width - 20;
      position.width -= width;

      EditorGUIUtility.IconContent("Animation.FilterBySelection");

      EditorGUI.BeginDisabledGroup(entry == null);
      if (GUI.Button(position, Styles.SelectIcon, Styles.IconButton)
        && entry != null) {
        if (Event.current.button == 0) {
          TypewriterUtils.Events.OnEntrySelected(entry);
        } else {
          fetch = true;
        }
      }

      EditorGUI.EndDisabledGroup();

      position.x += position.width;
      position.width = width;

      return fetch;
    }

    public static class Styles {
      public static readonly GUIContent SelectIcon =
        EditorGUIUtility.IconContent("Animation.FilterBySelection");

      public static readonly GUIStyle IconButton = new("MiniButton") {
        padding = new RectOffset(0, 0, 0, 0),
      };
    }
  }
}
