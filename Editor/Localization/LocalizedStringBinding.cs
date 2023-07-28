#if UNITY_LOCALIZATION
using System;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Localization {
  public class LocalizedStringBinding : IBinding {
    private const string GuidTag = "GUID:";

    private SerializedProperty _collectionName;
    private StringTableEntry _entry;
    private SerializedProperty _entryKeyId;

    private string _lastCollectionName;
    private string _lastEntryKey;
    private long _lastEntryKeyId;
    private VisualElement _parent;
    private SharedTableData.SharedTableEntry _sharedTableEntry;

    private StringTable _table;

    public LocalizedStringBinding(VisualElement parent) {
      _parent = parent;
      LocalizationEditorSettings.EditorEvents.TableEntryModified +=
        HandleTableEntryModified;
    }

    public SerializedProperty Property {
      set {
        _collectionName = value?.FindPropertyRelative("m_TableReference")
          .FindPropertyRelative("m_TableCollectionName");

        _entryKeyId = value?.FindPropertyRelative("m_TableEntryReference")
          .FindPropertyRelative("m_KeyId");

        Update();
      }
    }

    public string Value {
      get => _entry?.Value;
      set {
        if (_table == null || _entry == null || _sharedTableEntry == null) {
          return;
        }

        Undo.RecordObject(_table, "Set value");
        EditorUtility.SetDirty(_table);
        _entry.Value = value;
        LocalizationEvents.RaiseTableEntryModified(_sharedTableEntry);
      }
    }

    public string Key {
      get => _sharedTableEntry?.Key;
      set {
        if (_table == null || _sharedTableEntry == null) {
          return;
        }

        Undo.RecordObject(_table.SharedData, "Rename key");
        EditorUtility.SetDirty(_table.SharedData);
        _table.SharedData.RenameKey(_sharedTableEntry.Id, value);
      }
    }

    public void PreUpdate() { }

    public void Update() {
      if (_lastEntryKeyId == _entryKeyId?.longValue
        && _lastCollectionName == _collectionName?.stringValue
        && _lastEntryKey == Key) {
        return;
      }

      _lastEntryKeyId = _entryKeyId?.longValue ?? SharedTableData.EmptyId;
      _lastCollectionName = _collectionName?.stringValue;

      var tableCollection =
        LocalizationEditorSettings.GetStringTableCollection(
          GetTableReference()
        );
      if (tableCollection == null || tableCollection.StringTables.Count == 0) {
        _table = null;
        _entry = null;
        _sharedTableEntry = null;
        _lastEntryKey = null;

        return;
      }

      _table = tableCollection.StringTables[0];
      _entry = _table.GetEntry(_lastEntryKeyId);
      if (_lastEntryKeyId != SharedTableData.EmptyId && _entry == null) {
        _entry = _table.AddEntry(_lastEntryKeyId, string.Empty);
      }

      _sharedTableEntry = _table.SharedData.GetEntry(_lastEntryKeyId);
      _lastEntryKey = Key;

      Changed?.Invoke();
    }

    public void Release() { }

    public event Action Changed;

    private TableReference GetTableReference() {
      var value = _collectionName?.stringValue;

      if (!string.IsNullOrEmpty(value)
        && value.StartsWith(GuidTag, StringComparison.Ordinal)
        && Guid.TryParse(
          value.Substring(GuidTag.Length, value.Length - GuidTag.Length),
          out var guid
        )) {
        return guid;
      }

      return value;
    }

    ~LocalizedStringBinding() {
      LocalizationEditorSettings.EditorEvents.TableEntryModified -=
        HandleTableEntryModified;
    }

    private void HandleTableEntryModified(
      SharedTableData.SharedTableEntry entry
    ) {
      if (_sharedTableEntry?.Id == entry.Id) {
        Changed?.Invoke();
      }
    }
  }
}
#endif
