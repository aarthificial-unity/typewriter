using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  [CustomEntryDescriptor(typeof(EventEntry))]
  public class EventEntryDescriptor : EntryDescriptor {
    public override string Name => "Event";
    public override EntryType Type => EntryType.Event;
    public override string Color => "#8bc34a";
    public override bool Optional => true;

    public override void CreateNextMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) {
      var rule = (EventEntry)entry;

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
  }
}
