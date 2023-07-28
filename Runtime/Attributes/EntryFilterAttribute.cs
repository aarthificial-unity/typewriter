using Aarthificial.Typewriter.Entries;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Attributes {
  public class EntryFilterAttribute : PropertyAttribute {
    public EntryType Type { get; set; } =
      EntryType.Fact | EntryType.Event | EntryType.Rule;
    public EntryType PreferredType { get; set; } =
      EntryType.Fact | EntryType.Event | EntryType.Rule;
    public string TableName { get; set; }
    public bool AllowEmpty { get; set; }
    public Type Base { get; set; }

    public EntryFilterAttribute Copy() {
      return (EntryFilterAttribute)MemberwiseClone();
    }
  }
}
