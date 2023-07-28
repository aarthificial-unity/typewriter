#if UNITY_LOCALIZATION
using System.Reflection;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;

namespace Aarthificial.Typewriter.Editor.Localization {
  public static class LocalizationEvents {
    public static void RaiseTableEntryModified(object tableEntry) {
      var method = typeof(LocalizationEditorEvents).GetMethod(
        "RaiseTableEntryModified",
        BindingFlags.Instance | BindingFlags.NonPublic
      );

      method?.Invoke(
        LocalizationEditorSettings.EditorEvents,
        new[] { tableEntry }
      );
    }

    public static void RaiseTableEntryAdded(
      LocalizationTableCollection collection,
      SharedTableData.SharedTableEntry entry
    ) {
      var method = typeof(LocalizationEditorEvents).GetMethod(
        "RaiseTableEntryAdded",
        BindingFlags.Instance | BindingFlags.NonPublic
      );

      method?.Invoke(
        LocalizationEditorSettings.EditorEvents,
        new object[] { collection, entry }
      );
    }
  }
}
#endif
