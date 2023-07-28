using Aarthificial.Typewriter.Entries;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  [CustomEntryDescriptor(typeof(ScopeEntry))]
  public class ScopeEntryDescriptor : FactEntryDescriptor {
    public override string Name => "Scope";
  }
}
