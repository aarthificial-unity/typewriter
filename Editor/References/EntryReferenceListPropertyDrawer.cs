using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.References {
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
      previewPosition.x += EditorGUIUtility.labelWidth + 2;
      previewPosition.width -= EditorGUIUtility.labelWidth + 52;

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
      EntryReferencePropertyDrawer.RecreateRelations = true;
      if (isHover) {
        Event.current.Use();
      }

      var length = listProperty.arraySize;
      EditorGUI.PropertyField(
        position,
        listProperty,
        new GUIContent(property.displayName),
        true
      );
      if (length != listProperty.arraySize) {
        listProperty.serializedObject.ApplyModifiedProperties();
        TypewriterUtils.RecreateRelations();
      }

      if (isHover) {
        Event.current = current;
      }

      var newId = EntryReferencePropertyDrawer.CollectionEntryPicker(
        previewPosition,
        firstId,
        id => SetID(id, listProperty),
        previewPosition,
        _filter
      );

      if (newId != firstId) {
        SetID(newId, listProperty);
      }

      EntryReferencePropertyDrawer.RecreateRelations = false;
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

      listProperty.serializedObject.ApplyModifiedProperties();
      TypewriterUtils.RecreateRelations();
    }
  }
}
