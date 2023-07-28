using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.Extensions {
  public static class SerializedPropertyExtensions {
    internal static int FindEntryID(this SerializedProperty property) {
      return property.FindPropertyRelative(nameof(BaseEntry.ID))?.intValue ?? 0;
    }

    internal static BaseEntry FindEntry(this SerializedProperty property) {
      TypewriterDatabase.Instance.TryGetEntry(
        property.FindEntryID(),
        out var entry
      );
      return entry;
    }

    internal static bool Update(
      this SerializedProperty property,
      ref SerializedProperty reference
    ) {
      var same = reference == property;
      reference = property;
      return !same;
    }

    internal static SerializedProperty FirstString(
      this SerializedProperty property
    ) {
      if (property.propertyType == SerializedPropertyType.String) {
        return property;
      }

      var child = property.Copy();
      if (child.Next(true)) {
        do {
          if (child.propertyType == SerializedPropertyType.String) {
            return child;
          }
        } while (child.Next(false));
      }

      return null;
    }
  }
}
