using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Editor.References;
using System;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(BlackboardCriterion))]
  public class CriterionPropertyDrawer : PropertyDrawer {
    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var referenceProperty = property.FindPropertyRelative(
        nameof(BlackboardCriterion.FactReference)
      );
      var minProperty = property.FindPropertyRelative(
        nameof(BlackboardCriterion.Min)
      );
      var maxProperty = property.FindPropertyRelative(
        nameof(BlackboardCriterion.Max)
      );
      var typeProperty = property.FindPropertyRelative(
        nameof(BlackboardCriterion.Operation)
      );

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
      switch ((BlackboardCriterion.OperationType)typeProperty.enumValueIndex) {
        case BlackboardCriterion.OperationType.EqualReference:
          var referenceValue =
            EntryReferencePropertyDrawer.CollectionEntryPicker(
              position,
              minProperty.intValue,
              referenceValue => {
                minProperty.intValue = referenceValue;
                maxProperty.intValue = referenceValue;
                minProperty.serializedObject.ApplyModifiedProperties();
              },
              position,
              new EntryFilterAttribute()
            );
          if (referenceValue != minProperty.intValue) {
            minProperty.intValue = referenceValue;
            maxProperty.intValue = referenceValue;
          }

          break;
        case BlackboardCriterion.OperationType.Equal:
          var equalValue = EditorGUI.IntField(position, minProperty.intValue);
          if (EditorGUI.EndChangeCheck()) {
            minProperty.intValue = equalValue;
            maxProperty.intValue = equalValue;
          }

          break;
        case BlackboardCriterion.OperationType.GreaterEqual:
          var greaterEqualValue = EditorGUI.IntField(
            position,
            minProperty.intValue
          );
          if (EditorGUI.EndChangeCheck()) {
            minProperty.intValue = greaterEqualValue;
            maxProperty.intValue = int.MaxValue;
          }

          break;
        case BlackboardCriterion.OperationType.LessEqual:
          var lessEqualValue =
            EditorGUI.IntField(position, maxProperty.intValue);
          if (EditorGUI.EndChangeCheck()) {
            minProperty.intValue = int.MinValue;
            maxProperty.intValue = lessEqualValue;
          }

          break;
        case BlackboardCriterion.OperationType.ClosedInterval:
          position.width /= 2;
          var minClosedValue =
            EditorGUI.IntField(position, minProperty.intValue);
          position.x += position.width + 2;
          position.width -= 2;
          var maxClosedValue =
            EditorGUI.IntField(position, maxProperty.intValue);
          if (EditorGUI.EndChangeCheck()) {
            minProperty.intValue = minClosedValue;
            maxProperty.intValue = maxClosedValue;
          }

          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      EditorGUI.EndProperty();
    }
  }
}
