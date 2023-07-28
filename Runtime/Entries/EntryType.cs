using System;

namespace Aarthificial.Typewriter.Entries {
  [Flags]
  public enum EntryType {
    Fact = 1,
    Event = 2,
    Rule = 4,
  }
}
