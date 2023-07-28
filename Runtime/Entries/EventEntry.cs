using Aarthificial.Typewriter.Common;
using System;

namespace Aarthificial.Typewriter.Entries {
  [Serializable]
  public class EventEntry : BaseEntry {
    public override void AddToTable(DatabaseTable table) {
      table.Events.Add(this);
    }

    public override void RemoveFromTable(DatabaseTable table) {
      table.Events.Remove(this);
    }
  }
}
