using Aarthificial.Typewriter.Editor.PropertyDrawers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

/// <summary>
/// A property drawer for <see cref="DialogueEntry"/>.
/// </summary>
/// <remarks>
/// The Typewriter editor uses Unity drawers to display the entry details.
/// You can write your own drawers from scratch, or extend the base drawer.
///
/// In this example we exclude the `Text` property from the list of fields
/// drawn by the base drawer, and instead display it as a custom text field on
/// the very top.
/// </remarks>
[CustomPropertyDrawer(typeof(DialogueEntry))]
public class TextEntryPropertyDrawer : BaseEntryPropertyDrawer {
  protected override IEnumerable<string> GetHandledFields() =>
    base.GetHandledFields().Append(nameof(DialogueEntry.Text));

  protected override void PopulateContent(
    VisualElement root,
    SerializedProperty property
  ) {
    var text = new TextField {
      multiline = true,
      style = {
        height = 80,
        marginBottom = 8,
        whiteSpace = WhiteSpace.Normal,
      },
    };
    text.BindProperty(
      property.FindPropertyRelative(nameof(DialogueEntry.Text))
    );

    root.Add(text);
    base.PopulateContent(root, property);
  }
}
