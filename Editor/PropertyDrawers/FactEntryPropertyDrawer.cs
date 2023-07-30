using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(FactEntry), true)]
  public class FactEntryPropertyDrawer : BaseEntryPropertyDrawer {
    protected override IEnumerable<string> GetHandledFields() {
      return base.GetHandledFields()
        .Append(nameof(BaseEntry.Triggers))
        .Append(nameof(BaseEntry.Once))
        .Append(nameof(BaseEntry.Padding));
    }
  }
}
