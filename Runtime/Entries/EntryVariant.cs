using System;

namespace Aarthificial.Typewriter.Entries {
  [Flags]
  public enum EntryVariant {
    Fact = 1,
    Event = 2,
    Rule = 4,
  }
}
