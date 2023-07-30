using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aarthificial.Typewriter.Editor.Extensions {
  public static class CollectionCategoryExtensions {
    internal static List<BaseEntry> GetEntriesOfType(
      this DatabaseTable table,
      EntryVariant variant
    ) {
      return variant switch {
        EntryVariant.Fact => table.Facts.Cast<BaseEntry>().ToList(),
        EntryVariant.Rule => table.Rules.Cast<BaseEntry>().ToList(),
        EntryVariant.Event => table.Events.Cast<BaseEntry>().ToList(),
        _ => throw new ArgumentOutOfRangeException(
          nameof(variant),
          variant,
          null
        ),
      };
    }
  }
}
