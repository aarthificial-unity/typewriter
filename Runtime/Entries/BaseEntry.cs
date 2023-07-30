using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.References;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Typewriter.Entries {
  [Serializable]
  public abstract class BaseEntry : IComparable<BaseEntry> {
    public int ID;
    public string Key;

    [EntryFilter(
      Variant = EntryVariant.Fact,
      BaseType = typeof(ScopeEntry),
      AllowEmpty = true
    )]
    public EntryReference Scope;

    [SerializeField]
    public TriggerList Triggers =
      new() { List = Array.Empty<EntryReference>() };

    public bool Once;

    [SerializeField] public int Padding;

    [SerializeField]
    public BlackboardCriterion[] Criteria = Array.Empty<BlackboardCriterion>();

    [SerializeField]
    public BlackboardModification[] Modifications =
      Array.Empty<BlackboardModification>();

    [NonSerialized] public List<BaseEntry> Entries;

    public int Weight => Criteria.Length + Padding;

    public int CompareTo(BaseEntry other) {
      return other.Weight.CompareTo(Weight);
    }

    // TODO Move to descriptors.
    public abstract void AddToTable(DatabaseTable table);
    public abstract void RemoveFromTable(DatabaseTable table);

    public override string ToString() {
      return $"{Key} ({ID})";
    }

    public virtual void Apply(ITypewriterContext context) {
      TypewriterDatabase.Instance.MarkChange();

      for (var i = 0; i < Modifications.Length; i++) {
        var modification = Modifications[i];
        context.Apply(modification);
      }

      if (Scope.HasValue) {
        context.Add(this, 1);
      }
    }

    public virtual bool Test(ITypewriterContext context) {
      if (Once && Scope.HasValue && context.Get(this) != 0) {
        return false;
      }

      for (var i = 0; i < Criteria.Length; i++) {
        var criterion = Criteria[i];
        if (!context.Test(criterion)) {
          return false;
        }
      }

      return true;
    }

    public virtual void Invoke(ITypewriterContext context) {
      TypewriterDatabase.Instance.OnEntryEvent(context, this);
    }

    [Serializable]
    public struct TriggerList {
      public EntryReference[] List;
    }
  }
}
