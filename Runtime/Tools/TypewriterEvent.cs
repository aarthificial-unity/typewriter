using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Tools {
  [Serializable]
  public class TypewriterEvent {
    [SerializeField]
    [EntryFilter(PreferredType = EntryType.Event, AllowEmpty = true)]
    public EntryReference EventReference;

    public bool Invoke(ITypewriterContext provider) {
      if (EventReference.TryGetEntry(out var entry) && entry.Test(provider)) {
        return provider.Process(entry);
      }

      return false;
    }
  }
}
