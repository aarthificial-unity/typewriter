using Aarthificial.Typewriter.Entries;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(RuleEntry.Dispatcher))]
  public class DispatcherPropertyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(
      SerializedProperty property,
      GUIContent label
    ) {
      var referenceProperty =
        property.FindPropertyRelative(nameof(RuleEntry.Dispatcher.Reference));
      return EditorGUI.GetPropertyHeight(referenceProperty);
    }

    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var referenceProperty =
        property.FindPropertyRelative(nameof(RuleEntry.Dispatcher.Reference));

      EditorGUI.BeginProperty(position, label, property);
      EditorGUI.PropertyField(
        position,
        referenceProperty,
        GUIContent.none,
        true
      );
      EditorGUI.EndProperty();
    }
  }
}
