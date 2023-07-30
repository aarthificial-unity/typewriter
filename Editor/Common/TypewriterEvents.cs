using Aarthificial.Typewriter.Entries;
using System;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.Common {
  public class TypewriterEvents {
    public event Action<BaseEntry> EntrySelected;
    public event Action<DatabaseTable> TableSelected;
    public event Action LookupCreated;
    public event Action DatabaseRemoved;
    public event Action DatabaseCreated;

    public void OnLookupCreated() {
      LookupCreated?.Invoke();
    }

    public void OnEntrySelected(int id) {
      if (id != 0
        && TypewriterDatabase.Instance != null
        && TypewriterDatabase.Instance.TryGetEntry(id, out var entry)) {
        OnEntrySelected(entry);
      }
    }

    public void OnEntrySelected(BaseEntry entry) {
      EditorWindow.FocusWindowIfItsOpen<TypewriterEditor>();
      EntrySelected?.Invoke(entry);
    }

    public void OnDatabaseRemoved() {
      TypewriterDatabase.HasCachedInstance = false;
      DatabaseRemoved?.Invoke();
    }

    public void OnDatabaseCreated() {
      TypewriterDatabase.HasCachedInstance = false;
      DatabaseCreated?.Invoke();
    }

    public void OnTableSelected(DatabaseTable table) {
      EditorWindow.FocusWindowIfItsOpen<TypewriterEditor>();
      TableSelected?.Invoke(table);
    }
  }
}
