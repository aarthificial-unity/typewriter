using Aarthificial.Typewriter.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public class TableListItem : LabelListItem {
    private DatabaseTable _table;

    protected override void HandleFocusOut(FocusOutEvent evt) {
      base.HandleFocusOut(evt);
      Undo.RecordObject(_table, "Change table name");
      _table.TableName = Text.value;
    }

    public override void BindProperty(SerializedProperty property) {
      _table = (DatabaseTable)property.objectReferenceValue;
      Text.value = _table.TableName;
      SetLabel(_table.TableName);
    }
  }
}
