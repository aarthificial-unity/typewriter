using Aarthificial.Typewriter.Common;
using System;

namespace Aarthificial.Typewriter.Entries {
  [Serializable]
  public class FactEntry : BaseEntry {
    public override void AddToTable(DatabaseTable table) {
      table.Facts.Add(this);
    }

    public override void RemoveFromTable(DatabaseTable table) {
      table.Facts.Remove(this);
    }
  }
}
