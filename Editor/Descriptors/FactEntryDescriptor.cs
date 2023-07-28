using Aarthificial.Typewriter.Entries;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  [CustomEntryDescriptor(typeof(FactEntry))]
  public class FactEntryDescriptor : EntryDescriptor {
    public override string Name => "Fact";
    public override EntryType Type => EntryType.Fact;
    public override string Color => "#ff9800";
    public override bool Optional => false;
    public override bool HasCustomization => false;
  }
}
