using Aarthificial.Typewriter.Blackboards;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(Blackboard.Pair))]
  public class BlackboardPairPropertyDrawer : PropertyDrawer {
    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var referenceProperty = property.FindPropertyRelative(
        nameof(Blackboard.Pair.Key)
      );
      var valueProperty = property.FindPropertyRelative(
        nameof(Blackboard.Pair.Value)
      );

      EditorGUI.BeginProperty(position, label, property);

      var width = position.width * 0.35f;
      position.width -= width;
      EditorGUI.PropertyField(position, referenceProperty, GUIContent.none);

      position.x += position.width + 2;
      position.width = width - 2;
      EditorGUI.PropertyField(position, valueProperty, GUIContent.none);

      EditorGUI.EndProperty();
    }
  }
}
