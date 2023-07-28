using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Editor.References;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(BaseEntry.TriggerList))]
  public class EntryReferenceListPropertyDrawer : PropertyDrawer {
    private static readonly EntryFilterAttribute _filter = new() {
      AllowEmpty = true,
    };

    public override float GetPropertyHeight(
      SerializedProperty property,
      GUIContent label
    ) {
      var contextProperty =
        property.FindPropertyRelative(nameof(BaseEntry.TriggerList.List));
      return EditorGUI.GetPropertyHeight(contextProperty);
    }

    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var listProperty =
        property.FindPropertyRelative(nameof(BaseEntry.TriggerList.List));
      var firstId = 0;
      if (listProperty.arraySize > 0) {
        firstId = listProperty.GetArrayElementAtIndex(0)
          .FindPropertyRelative(nameof(EntryReference.InternalID))
          .intValue;
      }

      var previewPosition = position;
      previewPosition.height = 24;
      previewPosition.x += EditorGUIUtility.labelWidth;
      previewPosition.width -= EditorGUIUtility.labelWidth + 50;

      if (EntryReferencePropertyDrawer.DropEntry(
          previewPosition,
          _filter,
          out var entry
        )) {
        SetID(entry.ID, listProperty);
      }

      var isHover = Event.current.type == EventType.MouseDown
        && previewPosition.Contains(Event.current.mousePosition);
      var current = new Event(Event.current);

      EditorGUI.BeginProperty(position, label, property);
      if (isHover) {
        Event.current.Use();
      }

      EditorGUI.PropertyField(
        position,
        listProperty,
        new GUIContent(property.displayName),
        true
      );
      if (isHover) {
        Event.current = current;
      }

      EntryReferencePropertyDrawer.CollectionEntryPicker(
        previewPosition,
        firstId,
        id => {
          SetID(id, listProperty);
          property.serializedObject.ApplyModifiedProperties();
        },
        previewPosition,
        _filter
      );

      EditorGUI.EndProperty();
    }

    private static void SetID(int id, SerializedProperty listProperty) {
      if (id != 0) {
        if (listProperty.arraySize == 0) {
          listProperty.InsertArrayElementAtIndex(0);
        }

        listProperty.GetArrayElementAtIndex(0)
          .FindPropertyRelative(nameof(EntryReference.InternalID))
          .intValue = id;
      } else if (listProperty.arraySize > 0) {
        listProperty.DeleteArrayElementAtIndex(0);
      }
    }
  }
}
