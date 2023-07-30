using Aarthificial.Typewriter.Entries;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Attributes {
  public class EntryFilterAttribute : PropertyAttribute {
    public EntryVariant Variant { get; set; } = EntryVariant.Fact
      | EntryVariant.Event
      | EntryVariant.Rule;
    public EntryVariant PreferredVariant { get; set; } = EntryVariant.Fact
      | EntryVariant.Event
      | EntryVariant.Rule;
    public string TableName { get; set; }
    public bool AllowEmpty { get; set; }
    public Type BaseType { get; set; }

    public EntryFilterAttribute Copy() {
      return (EntryFilterAttribute)MemberwiseClone();
    }
  }
}
