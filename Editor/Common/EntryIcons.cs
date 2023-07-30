using Aarthificial.Typewriter.Entries;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.Common {
  internal static class EntryIcons {
    private static readonly Texture2D EventEntryIcon =
      Resources.Load<Texture2D>("Textures/EventEntry");
    private static readonly Texture2D FactEntryIcon =
      Resources.Load<Texture2D>("Textures/FactEntry");
    private static readonly Texture2D RuleEntryIcon =
      Resources.Load<Texture2D>("Textures/RuleEntry");

    public static Texture2D GetIcon(this EntryVariant variant) {
      return variant switch {
        EntryVariant.Event => EventEntryIcon,
        EntryVariant.Fact => FactEntryIcon,
        _ => RuleEntryIcon,
      };
    }
  }
}
