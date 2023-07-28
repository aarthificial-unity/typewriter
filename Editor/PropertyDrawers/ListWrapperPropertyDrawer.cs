using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(Blackboard))]
  [CustomPropertyDrawer(typeof(TypewriterModification))]
  [CustomPropertyDrawer(typeof(TypewriterCriteria))]
  public class ListWrapperPropertyDrawer : PropertyDrawer {
    private const string _blackboard = nameof(Blackboard.List);
    private const string _modification = nameof(TypewriterModification.List);
    private const string _criteria = nameof(TypewriterCriteria.List);

    public override float GetPropertyHeight(
      SerializedProperty property,
      GUIContent label
    ) {
      Assert.AreEqual(_blackboard, _modification);
      Assert.AreEqual(_criteria, _modification);

      var listProperty = property.FindPropertyRelative(_modification);
      return EditorGUI.GetPropertyHeight(listProperty);
    }

    public override void OnGUI(
      Rect position,
      SerializedProperty property,
      GUIContent label
    ) {
      var criteriaProperty = property.FindPropertyRelative(_modification);
      EditorGUI.BeginProperty(position, label, property);
      EditorGUI.PropertyField(position, criteriaProperty, label, true);
      EditorGUI.EndProperty();
    }
  }
}
