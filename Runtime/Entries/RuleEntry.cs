using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.References;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Aarthificial.Typewriter.Entries {
  [Serializable]
  public class RuleEntry : BaseEntry {
    [Serializable]
    public struct Dispatcher {
      [EntryFilter(PreferredType = EntryType.Event)]
      public EntryReference Reference;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Dispatch(
      ITypewriterContext context,
      Dispatcher[] dispatchers
    ) {
      for (var i = 0; i < dispatchers.Length; i++) {
        var reference = dispatchers[i].Reference;
        if (reference.TryGetEntry(out var entry) && entry.Test(context)) {
          entry.Invoke(context);
        }
      }
    }

    [SerializeField] public Dispatcher[] OnApply = Array.Empty<Dispatcher>();
    [SerializeField] public Dispatcher[] OnInvoke = Array.Empty<Dispatcher>();

    public override void Apply(ITypewriterContext context) {
      base.Apply(context);
      Dispatch(context, OnApply);
    }

    public override void Invoke(ITypewriterContext context) {
      Dispatch(context, OnInvoke);
      base.Invoke(context);
    }

    public override void AddToTable(DatabaseTable table) {
      table.Rules.Add(this);
    }

    public override void RemoveFromTable(DatabaseTable table) {
      table.Rules.Remove(this);
    }
  }
}
