using Aarthificial.Typewriter.Tools;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(TypewriterEvent))]
  public class TypewriterEventPropertyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(
      SerializedProperty property,
      GUIContent label
    ) {
      var eventProperty = property.FindPropertyRelative(
        nameof(TypewriterEvent.EventReference)
      );
      return EditorGUI.GetPropertyHeight(eventProperty);
    }

    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var eventProperty = property.FindPropertyRelative(
        nameof(TypewriterEvent.EventReference)
      );

      EditorGUI.BeginProperty(position, label, property);
      EditorGUI.PropertyField(position, eventProperty, GUIContent.none, true);
      EditorGUI.EndProperty();
    }
  }
}
