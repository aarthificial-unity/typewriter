using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aarthificial.Typewriter.Editor.Extensions {
  public static class CollectionCategoryExtensions {
    internal static List<BaseEntry> GetEntriesOfType(
      this DatabaseTable table,
      EntryType type
    ) {
      return type switch {
        EntryType.Fact => table.Facts.Cast<BaseEntry>().ToList(),
        EntryType.Rule => table.Rules.Cast<BaseEntry>().ToList(),
        EntryType.Event => table.Events.Cast<BaseEntry>().ToList(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
      };
    }
  }
}
