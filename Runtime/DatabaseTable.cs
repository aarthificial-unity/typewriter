using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Typewriter {
  [Serializable]
  public class DatabaseTable : ScriptableObject {
    public string TableName;
    [SerializeReference] public List<RuleEntry> Rules = new();
    [SerializeReference] public List<FactEntry> Facts = new();
    [SerializeReference] public List<EventEntry> Events = new();

    public virtual void RemoveEntry(BaseEntry entry) {
      entry.RemoveFromTable(this);
    }

    public virtual void AddEntry(BaseEntry entry) {
      entry.AddToTable(this);
    }

    public virtual void Setup(TypewriterDatabase database) { }
  }
}
