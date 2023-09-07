using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Tools;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aarthificial.Typewriter {
  public static class TypewriterContextExtensions {
    public static bool WouldInvoke(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      return reference.TryGetEntry(out var entry)
        && provider.WouldInvoke(entry);
    }

    public static bool WouldInvoke(
      this ITypewriterContext provider,
      BaseEntry entry
    ) {
      return entry.Test(provider) && provider.HasMatchingRule(entry.ID);
    }

    public static bool TryInvoke(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      return reference.TryGetEntry(out var entry) && provider.TryInvoke(entry);
    }

    public static bool TryInvoke(
      this ITypewriterContext provider,
      BaseEntry entry
    ) {
      if (entry.Test(provider)) {
        return provider.Process(entry);
      }

      return false;
    }

    public static bool Process(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      return reference.TryGetEntry(out var entry) && provider.Process(entry);
    }

    /// <summary></summary>
    /// <param name="provider"></param>
    /// <param name="entry"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public static bool Process(
      this ITypewriterContext provider,
      BaseEntry entry
    ) {
      entry.Apply(provider);
      if (!provider.FindMatchingRule(entry.ID, out var match)) {
        return false;
      }
      match.Invoke(provider);
      return true;
    }

    public static void Set(
      this ITypewriterContext provider,
      BaseEntry entry,
      int value
    ) {
      if (provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        TypewriterDatabase.Instance.MarkChange();
        blackboard.Set(entry.ID, value);
      }
    }

    public static void Set(
      this ITypewriterContext provider,
      EntryReference reference,
      int value
    ) {
      if (reference.TryGetEntry(out var entry)
        && provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        TypewriterDatabase.Instance.MarkChange();
        blackboard.Set(entry.ID, value);
      }
    }

    public static void Add(
      this ITypewriterContext provider,
      BaseEntry entry,
      int value
    ) {
      if (provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        TypewriterDatabase.Instance.MarkChange();
        blackboard.Add(entry.ID, value);
      }
    }

    public static void Add(
      this ITypewriterContext provider,
      EntryReference reference,
      int value
    ) {
      if (reference.TryGetEntry(out var entry)
        && provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        TypewriterDatabase.Instance.MarkChange();
        blackboard.Add(entry.ID, value);
      }
    }

    public static int Get(this ITypewriterContext provider, BaseEntry entry) {
      return provider.TryGetBlackboard(entry.Scope, out var blackboard)
        ? blackboard.Get(entry.ID)
        : 0;
    }

    public static int Get(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      return reference.TryGetEntry(out var entry)
        && provider.TryGetBlackboard(entry.Scope, out var blackboard)
          ? blackboard.Get(entry.ID)
          : 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Test(
      this ITypewriterContext provider,
      BlackboardCriterion criterion
    ) {
      if (criterion.FactReference.TryGetEntry(out var entry)
        && provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        return blackboard.Test(criterion);
      }

      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Test(
      this ITypewriterContext provider,
      TypewriterCriteria criteria
    ) {
      for (var i = 0; i < criteria.List.Length; i++) {
        var criterion = criteria.List[i];
        if (!Test(provider, criterion)) {
          return false;
        }
      }

      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Apply(
      this ITypewriterContext provider,
      BlackboardModification modification
    ) {
      if (modification.FactReference.TryGetEntry(out var entry)
        && provider.TryGetBlackboard(entry.Scope, out var blackboard)) {
        TypewriterDatabase.Instance.MarkChange();
        blackboard.Modify(modification);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Apply(
      this ITypewriterContext provider,
      TypewriterModification modifications
    ) {
      for (var i = 0; i < modifications.List.Length; i++) {
        var modification = modifications.List[i];
        Apply(provider, modification);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Apply(
      this ITypewriterContext provider,
      IList<BlackboardModification> modifications
    ) {
      for (var i = 0; i < modifications.Count; i++) {
        var modification = modifications[i];
        Apply(provider, modification);
      }
    }

    public static bool HasMatchingRule(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      if (!reference.TryGetEntry(out var entry)) {
        return false;
      }

      for (var i = 0; i < entry.Entries.Count; i++) {
        var response = entry.Entries[i];
        if (response.Test(provider)) {
          return true;
        }
      }

      return false;
    }

    public static int CountMatchingRules(
      this ITypewriterContext provider,
      EntryReference reference
    ) {
      if (!reference.TryGetEntry(out var entry)) {
        return 0;
      }

      var count = 0;
      for (var i = 0; i < entry.Entries.Count; i++) {
        var response = entry.Entries[i];
        if (response.Test(provider)) {
          count++;
        }
      }

      return count;
    }

    public static int FindMatchingRules(
      this ITypewriterContext provider,
      EntryReference reference,
      BaseEntry[] matches
    ) {
      if (!reference.TryGetEntry(out var entry)) {
        return 0;
      }

      var count = 0;
      for (var i = 0; i < entry.Entries.Count; i++) {
        var response = entry.Entries[i];
        if (response.Test(provider)) {
          matches[count++] = response;
        }
      }

      return count;
    }

    public static int FindMatchingRules<T>(
      this ITypewriterContext provider,
      EntryReference reference,
      T[] matches
    ) where T : BaseEntry {
      if (!reference.TryGetEntry(out var entry)) {
        return 0;
      }

      var count = 0;
      for (var i = 0; i < entry.Entries.Count; i++) {
        var response = entry.Entries[i];
        if (response.Test(provider) && response is T typed) {
          matches[count++] = typed;
        }
      }

      return count;
    }

    public static bool FindMatchingRule(
      this ITypewriterContext provider,
      EntryReference reference,
      out BaseEntry match
    ) {
      match = null;
      if (!reference.TryGetEntry(out var entry)) {
        return false;
      }

      for (var i = 0; i < entry.Entries.Count; i++) {
        var response = entry.Entries[i];
        if (response.Test(provider)) {
          match = response;
          return true;
        }
      }

      return false;
    }
  }
}
