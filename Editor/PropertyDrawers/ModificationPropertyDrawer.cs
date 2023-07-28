using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Editor.References;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(BlackboardModification))]
  public class ModificationPropertyDrawer : PropertyDrawer {
    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var referenceProperty =
        property.FindPropertyRelative(
          nameof(BlackboardModification.FactReference)
        );
      var typeProperty =
        property.FindPropertyRelative(nameof(BlackboardModification.Operation));
      var valueProperty =
        property.FindPropertyRelative(nameof(BlackboardModification.Value));

      EditorGUI.BeginProperty(position, label, property);

      var width = position.width * 0.35f;
      position.width -= width;
      EditorGUI.PropertyField(position, referenceProperty, GUIContent.none);

      position.x += position.width;
      position.width = 40;
      EditorGUI.BeginChangeCheck();
      EditorGUI.PropertyField(position, typeProperty, GUIContent.none);

      position.x += position.width + 2;
      position.width = width - 42;
      if (typeProperty.enumValueIndex
        == (int)BlackboardModification.OperationType.SetReference) {
        valueProperty.intValue =
          EntryReferencePropertyDrawer.CollectionEntryPicker(
            position,
            valueProperty.intValue,
            referenceValue => {
              valueProperty.intValue = referenceValue;
              valueProperty.serializedObject.ApplyModifiedProperties();
            },
            position,
            new EntryFilterAttribute()
          );
      } else {
        EditorGUI.PropertyField(position, valueProperty, GUIContent.none);
      }

      EditorGUI.EndProperty();
    }
  }
}
