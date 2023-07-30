using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  [CustomEntryDescriptor(typeof(RuleEntry))]
  public class RuleEntryDescriptor : EntryDescriptor {
    public override string Name => "Rule";
    public override EntryVariant Variant => EntryVariant.Rule;
    public override string Color => "#00bcd4";
    public override bool Optional => false;

    public override void CreatePreviousMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) {
      var rule = (RuleEntry)entry;
      TypewriterDatabase.Instance.TryGetTable(rule.ID, out var mainTable);
      foreach (var reference in rule.Triggers.List) {
        if (!reference.TryGetEntry(out var trigger)) {
          continue;
        }

        if (trigger.ID != 0
          && TypewriterDatabase.Instance.TryGetTable(
            trigger.ID,
            out var table
          )) {
          names.Add(
            mainTable == table
              ? trigger.GetKey()
              : $"{table.name}/{trigger.GetKey()}"
          );
          ids.Add(trigger.ID);
        }
      }
    }

    public override void CreateAlternativeMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) {
      var rule = (RuleEntry)entry;

      TypewriterDatabase.Instance.TryGetTable(rule.ID, out var mainTable);
      foreach (var reference in rule.Triggers.List) {
        if (!reference.TryGetEntry(out var triggerEntry)) {
          continue;
        }

        foreach (var response in triggerEntry.Entries) {
          if (response.ID != 0
            && TypewriterDatabase.Instance.TryGetTable(
              response.ID,
              out var table
            )) {
            if (response.ID == rule.ID) {
              current = names.Count;
            }

            names.Add(
              mainTable == table
                ? response.GetKey()
                : $"{table.name}/{response.GetKey()}"
            );
            ids.Add(response.ID);
          }
        }
      }
    }

    public override void CreateNextMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) {
      var rule = (RuleEntry)entry;
      TypewriterDatabase.Instance.TryGetTable(rule.ID, out var mainTable);
      foreach (var response in rule.Entries) {
        if (response.ID != 0
          && TypewriterDatabase.Instance.TryGetTable(
            response.ID,
            out var table
          )) {
          names.Add(
            mainTable == table
              ? response.GetKey()
              : $"{table.name}/{response.GetKey()}"
          );
          ids.Add(response.ID);
        }
      }
    }

    public override void HandleEntryCreated(
      BaseEntry entry,
      DatabaseTable table
    ) {
      var rule = (RuleEntry)entry;
      var index = table.Rules.IndexOf(rule);
      if (index > 0) {
        var previous = table.Rules[index - 1];
        ArrayUtility.Add(ref rule.Triggers.List, previous.ID);
        TypewriterDatabase.Instance.UpdateLookupIfExists();
      }
    }
  }
}
