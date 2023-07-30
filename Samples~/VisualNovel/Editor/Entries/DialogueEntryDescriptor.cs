using Aarthificial.Typewriter.Editor.Descriptors;

/// <summary>
/// A descriptor for <see cref="DialogueEntry"/>.
/// </summary>
/// <remarks>
/// Descriptors are used to control how custom entries are handled by the
/// Typewriter editor.
///
/// In this simple example we change the name of the entry to "Dialogue".
/// </remarks>
[CustomEntryDescriptor(typeof(DialogueEntry))]
public class TextEntryDescriptor : RuleEntryDescriptor {
  public override string Name => "Dialogue";
}
