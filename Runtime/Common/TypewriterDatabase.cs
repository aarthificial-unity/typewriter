using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aarthificial.Typewriter.Common {
  public class TypewriterDatabase : ScriptableObject,
    ISerializationCallbackReceiver {
    public delegate void EntryAction(
      BaseEntry entry,
      ITypewriterContext context
    );

    internal static bool HasCachedInstance;
    private static TypewriterDatabase _instance;

    public List<DatabaseTable> Tables = new();

    private readonly Dictionary<int, BaseEntry> _entryLookup = new();
    private readonly Dictionary<int, EntryAction> _events = new();
    private readonly Dictionary<int, List<BaseEntry>> _ruleLookup = new();
    private readonly Dictionary<int, DatabaseTable> _tableLookup = new();
    private bool _lookupCreated;
    private int _changeCounter = 1;

    public static TypewriterDatabase Instance {
      get {
#if UNITY_EDITOR
        if (!HasCachedInstance) {
          HasCachedInstance = true;
          var guids =
            AssetDatabase.FindAssets("t:" + nameof(TypewriterDatabase));
          if (guids.Length > 0) {
            _instance =
              (TypewriterDatabase)AssetDatabase.LoadMainAssetAtPath(
                AssetDatabase.GUIDToAssetPath(guids[0])
              );
          }
        }
#endif
        return _instance;
      }
    }

    private void OnEnable() {
      _changeCounter = 1;
      if (!HasCachedInstance) {
        HasCachedInstance = true;
        _instance = this;
      }
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize() {
      _lookupCreated = false;
    }

    private event EntryAction GlobalEvent;

    [RuntimeInitializeOnLoadMethod(
      RuntimeInitializeLoadType.SubsystemRegistration
    )]
    private static void Init() {
      Instance._lookupCreated = false;
    }

    private int GetNextKey() {
      var key = UidGenerator.GetNextKey();
      while (Contains(key)) {
        key = UidGenerator.GetNextKey();
      }

      return key;
    } // ReSharper disable Unity.PerformanceAnalysis
    public void CreateLookupIfNecessary() {
      if (!_lookupCreated) {
        CreateLookup();
      }
    }

    public void UpdateLookupIfExists() {
      if (_lookupCreated) {
        CreateLookup();
      }
    }

    public void MarkChange() {
      _changeCounter++;
    }

    public bool HasChangedSince(ref int change) {
      if (change < _changeCounter) {
        change = _changeCounter;
        return true;
      }

      return false;
    }

    public void CreateLookup() {
#if UNITY_EDITOR
      Debug.Log("Typewriter: Recreating database lookup", this);
#endif

      _lookupCreated = true;
      _ruleLookup.Clear();
      _entryLookup.Clear();
      _tableLookup.Clear();

      foreach (var table in Tables) {
#if UNITY_EDITOR
        if (table == null) {
          Debug.LogWarning("Typewriter: Skipping null table", this);
          continue;
        }
#endif

        foreach (var entry in table.Rules) {
          OnCreateEntry(entry, table);
        }

        foreach (var entry in table.Facts) {
          OnCreateEntry(entry, table);
        }

        foreach (var entry in table.Events) {
          OnCreateEntry(entry, table);
        }
      }
    }

    #region rules

    public List<BaseEntry> GetRules(int id) {
      CreateLookupIfNecessary();
      if (!_ruleLookup.TryGetValue(id, out var list)) {
        list = new List<BaseEntry>();
        _ruleLookup[id] = list;
      }

      return list;
    }

    #endregion

    #region entries

    public BaseEntry CreateEntry(DatabaseTable table, Type entryType) {
      var entry = (BaseEntry)Activator.CreateInstance(entryType);
      entry.ID = GetNextKey();
      AddEntry(table, entry);

      return entry;
    }

    public void AddEntry(DatabaseTable table, BaseEntry entry) {
      if (!Tables.Contains(table)) {
        throw new InvalidOperationException();
      }

      table.AddEntry(entry);
      OnCreateEntry(entry, table);
    }

    public bool RemoveEntry(BaseEntry entry) {
      if (TryGetTable(entry.ID, out var table)) {
        table.RemoveEntry(entry);
        OnRemoveEntry(entry);
        return true;
      }

      return false;
    }

    public bool RemoveEntry(int id) {
      return TryGetEntry(id, out var entry) && RemoveEntry(entry);
    }

    public bool TryGetEntry(int id, out BaseEntry entry) {
      CreateLookupIfNecessary();
      return _entryLookup.TryGetValue(id, out entry);
    }

    public bool TryGetEntry<T>(int id, out T entry) where T : BaseEntry {
      if (TryGetEntry(id, out var raw) && raw is T typed) {
        entry = typed;
        return true;
      }

      entry = default;
      return false;
    }

    public bool Contains(int id) {
      CreateLookupIfNecessary();
      return _entryLookup.ContainsKey(id);
    }

    private void OnCreateEntry(BaseEntry entry, DatabaseTable table) {
      if (entry == null) {
        Debug.LogWarningFormat(
          "Typewriter: Invalid entry in table {0}",
          table.name
        );
        return;
      }

      if (entry.ID == 0) {
        Debug.LogWarningFormat(
          "Typewriter: Dialogue {0} skipped (id == 0)",
          entry
        );
        return;
      }

      if (!_lookupCreated) {
        return;
      }

      _entryLookup[entry.ID] = entry;
      _tableLookup[entry.ID] = table;
      var id = entry.ID;
      entry.Entries = GetRules(entry.ID);

      foreach (var trigger in entry.Triggers.List) {
        var searchList = GetRules(trigger.ID);
        var index = searchList.BinarySearch(entry);
        if (index < 0) {
          index = ~index;
        }

        searchList.Insert(index, entry);
      }
    }

    private void OnRemoveEntry(BaseEntry entry) {
      if (!_lookupCreated) {
        return;
      }

      _entryLookup.Remove(entry.ID);
      _tableLookup.Remove(entry.ID);

      foreach (var trigger in entry.Triggers.List) {
        GetRules(trigger.ID).Remove(entry);
      }
    }

    #endregion

    #region tables

    public int AddTable(DatabaseTable table, int index = -1) {
      if (index > -1) {
        Tables.Insert(index, table);
        return index;
      }

      Tables.Add(table);
      return Tables.Count - 1;
    }

    public void RemoveTable(DatabaseTable table) {
      if (!Tables.Contains(table)) {
        return;
      }

      foreach (var entry in table.Rules) {
        RemoveEntry(entry);
      }

      Tables.Remove(table);
    }

    public bool TryGetTable(int id, out DatabaseTable table) {
      CreateLookupIfNecessary();
      return _tableLookup.TryGetValue(id, out table);
    }

    public bool TryGetTable<T>(int id, out T table) where T : DatabaseTable {
      if (TryGetTable(id, out var raw) && raw is T typed) {
        table = typed;
        return true;
      }

      table = default;
      return false;
    }

    public void MoveToTable(int id, DatabaseTable table) {
      if (TryGetTable(id, out var oldCategory)
        && oldCategory != table
        && TryGetEntry(id, out var entry)) {
        table.RemoveEntry(entry);
        table.AddEntry(entry);

        if (_lookupCreated) {
          _tableLookup[id] = table;
        }
      }
    }

    #endregion

    #region events

    public void AddListener(EntryAction action) {
      GlobalEvent += action;
    }

    public void RemoveListener(EntryAction action) {
      GlobalEvent -= action;
    }

    public void AddListener(int id, EntryAction action) {
      if (_events.ContainsKey(id)) {
        _events[id] += action;
      } else {
        _events[id] = action;
      }
    }

    public void RemoveListener(int id, EntryAction action) {
      if (!_events.ContainsKey(id)) {
        return;
      }

      var listeners = _events[id] - action;
      if (listeners == null) {
        _events.Remove(id);
      } else {
        _events[id] = listeners;
      }
    }

    public void OnEntryEvent(ITypewriterContext provider, BaseEntry entry) {
      Assert.IsNotNull(entry);
      GlobalEvent?.Invoke(entry, provider);
      if (_events.TryGetValue(entry.ID, out var callbacks)) {
        callbacks.Invoke(entry, provider);
      }
    }

    #endregion
  }
}
