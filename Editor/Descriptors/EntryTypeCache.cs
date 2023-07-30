using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  internal static class EntryTypeCache {
    private static readonly Dictionary<Type, EntryDescriptor> _lookup = new();

    [InitializeOnLoadMethod]
    private static void Initialize() {
      _lookup.Clear();

      var definitions = new Dictionary<Type, EntryDescriptor>();
      var descriptors =
        TypeCache.GetTypesWithAttribute<CustomEntryDescriptorAttribute>();
      foreach (var candidate in descriptors) {
        if (!candidate.IsSubclassOf(typeof(EntryDescriptor))) {
          continue;
        }

        var attributes = candidate.GetCustomAttributes(
          typeof(CustomEntryDescriptorAttribute),
          false
        );

        foreach (var attribute in attributes) {
          if (attribute is CustomEntryDescriptorAttribute descriptorAttribute) {
            var descriptor =
              Activator.CreateInstance(candidate) as EntryDescriptor;
            descriptor.RealType = descriptorAttribute.Type;
            definitions[descriptorAttribute.Type] = descriptor;
          }
        }
      }

      var entryTypes = TypeCache.GetTypesDerivedFrom<BaseEntry>();
      foreach (var entryType in entryTypes) {
        if (definitions.TryGetValue(entryType, out var descriptor)) {
          _lookup[entryType] = descriptor;
          continue;
        }

        var parent = entryType.BaseType;
        while (parent != null) {
          if (definitions.TryGetValue(parent, out descriptor)) {
            descriptor =
              Activator.CreateInstance(descriptor.GetType()) as EntryDescriptor;
            descriptor.RealType = entryType;
            _lookup[entryType] = descriptor;
            break;
          }

          parent = parent.BaseType;
        }
      }
    }

    public static List<EntryDescriptor> GetTypes(EntryVariant entryVariant) {
      var visibleTypes = new List<EntryDescriptor>();
      var optionalTypes = new List<EntryDescriptor>();

      foreach (var descriptor in _lookup.Values) {
        if (!entryVariant.HasFlag(descriptor.Variant)) {
          continue;
        }

        if (descriptor.Optional) {
          optionalTypes.Add(descriptor);
        } else {
          visibleTypes.Add(descriptor);
        }
      }

      return visibleTypes.Count > 0 ? visibleTypes : optionalTypes;
    }

    internal static bool TryGetDescriptor(
      BaseEntry entry,
      out EntryDescriptor descriptor
    ) {
      return _lookup.TryGetValue(entry.GetType(), out descriptor);
    }

    internal static bool TryGetDescriptor(
      Type type,
      out EntryDescriptor descriptor
    ) {
      return _lookup.TryGetValue(type, out descriptor);
    }

    internal static bool Test(
      this EntryFilterAttribute filter,
      BaseEntry entry
    ) {
      if (entry == null) {
        return filter.AllowEmpty;
      }

      if (!TryGetDescriptor(entry, out var descriptor)) {
        return false;
      }

      if (!filter.Variant.HasFlag(descriptor.Variant)) {
        return false;
      }

      if (filter.TableName != null
        && (!TypewriterDatabase.Instance.TryGetTable(entry.ID, out var table)
          || table.TableName != filter.TableName)) {
        return false;
      }

      if (filter.BaseType != null && !filter.BaseType.IsInstanceOfType(entry)) {
        return false;
      }

      return true;
    }

    internal static string GetName(this EntryFilterAttribute filter) {
      if (filter.BaseType == null) {
        return filter.Variant.ToString();
      }

      if (TryGetDescriptor(filter.BaseType, out var descriptor)) {
        return descriptor.Name;
      }

      return filter.BaseType.Name;
    }

    internal static BaseEntry FindFirstMatch(this EntryFilterAttribute filter) {
      if (TypewriterDatabase.Instance == null || filter.AllowEmpty) {
        return null;
      }

      foreach (var table in TypewriterDatabase.Instance.Tables) {
        if (filter.TableName != null && filter.TableName != table.TableName) {
          continue;
        }

        foreach (var type in filter.Variant.GetMatching()) {
          var entries = table.GetEntriesOfType(type);
          if (filter.BaseType != null) {
            foreach (var entry in entries) {
              if (filter.BaseType.IsInstanceOfType(entry)) {
                return entry;
              }
            }
          } else if (entries.Count > 0) {
            return entries[0];
          }
        }

        if (filter.TableName != null) {
          return null;
        }
      }

      return null;
    }
  }
}
