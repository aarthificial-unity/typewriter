using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.Common {
  public static class TypewriterUtils {
    public static readonly TypewriterEvents Events = new();

    [InitializeOnLoadMethod]
    public static void Initialize() {
      Undo.undoRedoPerformed -= HandleUndoRedoPerformed;
      Undo.undoRedoPerformed += HandleUndoRedoPerformed;
    }

    private static void HandleUndoRedoPerformed() {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      TypewriterDatabase.Instance.UpdateLookupIfExists();
      Events.OnLookupCreated();
    }

    public static void CreateDatabase() {
      if (TypewriterDatabase.Instance != null) {
        return;
      }

      var path = EditorUtility.SaveFilePanelInProject(
        "Save",
        "TypewriterDatabase",
        "asset",
        "Please enter a file name to save the database to"
      );

      if (string.IsNullOrEmpty(path)) {
        return;
      }

      var database = ScriptableObject.CreateInstance<TypewriterDatabase>();
      AssetDatabase.CreateAsset(database, path);
      Events.OnDatabaseCreated();
    }

    public static void RecreateLookup() {
      if (TypewriterDatabase.Instance == null) {
        return;
      }

      TypewriterDatabase.Instance.CreateLookup();
      Events.OnLookupCreated();
    }

    public static string GetKey(this BaseEntry entry) {
#if UNITY_LOCALIZATION
      if (entry is LocalizedRuleEntry textRule) {
        var tableCollection =
          UnityEditor.Localization.LocalizationEditorSettings
            .GetStringTableCollection(textRule.Text.TableReference);
        if (tableCollection == null) {
          return "<empty>";
        }

        return tableCollection.SharedData.GetKey(
            textRule.Text.TableEntryReference.KeyId
          )
          ?? "<empty>";
      }
#endif

      return entry.Key;
    }

    internal static IEnumerable<T> GetMatching<T>(this T filter)
      where T : Enum {
      foreach (T value in Enum.GetValues(typeof(T))) {
        if (filter.HasFlag(value)) {
          yield return value;
        }
      }
    }

    internal static bool TryParseType(string name, out Type type) {
      var segments = name.Split(' ');
      type = Type.GetType($"{segments[1]}, {segments[0]}");
      return type != null;
    }
  }
}
