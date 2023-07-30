using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Aarthificial.Typewriter.References {
  [Serializable]
  public struct EntryReference : IEquatable<EntryReference> {
    [SerializeField] internal int InternalID;

    public int ID {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => InternalID;
    }

    public bool HasValue {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => InternalID != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(EntryReference other) {
      return InternalID == other.InternalID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BaseEntry GetEntry() {
      TryGetEntry(out var entry);
      return entry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetEntry<T>() where T : BaseEntry {
      TryGetEntry(out T entry);
      return entry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetEntry(out BaseEntry entry) {
      return TypewriterDatabase.Instance.TryGetEntry(InternalID, out entry);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetEntry<T>(out T entry) where T : BaseEntry {
      return TypewriterDatabase.Instance.TryGetEntry(InternalID, out entry);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) {
      return obj is EntryReference other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() {
      return InternalID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(EntryReference lhs, EntryReference rhs) {
      return lhs.InternalID == rhs.InternalID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(EntryReference lhs, EntryReference rhs) {
      return lhs.InternalID != rhs.InternalID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(EntryReference reference) {
      return reference.InternalID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator EntryReference(int id) {
      return new EntryReference { InternalID = id };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator EntryReference(BaseEntry entry) {
      return new EntryReference { InternalID = entry?.ID ?? 0 };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BaseEntry(EntryReference reference) {
      return reference.GetEntry();
    }
  }
}
