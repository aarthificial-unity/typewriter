#if UNITY_LOCALIZATION
using Aarthificial.Typewriter.Editor.PropertyDrawers;
using Aarthificial.Typewriter.Entries;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Localization {
  [CustomPropertyDrawer(typeof(LocalizedRuleEntry), true)]
  public class LocalizedRulePropertyDrawer : BaseEntryPropertyDrawer {
    protected override void PopulateContent(
      VisualElement root,
      SerializedProperty property
    ) {
      root.Add(
        new LocalizedTextEditor(
          property.FindPropertyRelative(nameof(LocalizedRuleEntry.Text))
        ) {
          multiline = true,
          style = {
            height = 80,
            marginBottom = 8,
            whiteSpace = WhiteSpace.Normal,
          },
        }
      );
      base.PopulateContent(root, property);
    }
  }
}
#endif
