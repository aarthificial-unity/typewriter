using Aarthificial.Typewriter.Editor.Descriptors;

/// <summary>
/// A descriptor for <see cref="SpeakerEntry"/>.
/// </summary>
/// <remarks>
/// Descriptors are used to control how custom entries are handled by the
/// Typewriter editor.
///
/// In this example we change the display name to "Speaker" and the color to
/// a bright red.
/// </remarks>
[CustomEntryDescriptor(typeof(SpeakerEntry))]
public class SpeakerEntryDescriptor : FactEntryDescriptor {
  public override string Name => "Speaker";
  public override string Color => "#ff6224";
}
