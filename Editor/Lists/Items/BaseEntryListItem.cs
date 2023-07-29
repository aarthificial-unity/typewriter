using Aarthificial.Typewriter.Editor.Common;
using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Entries;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists.Items {
  public class BaseEntryListItem : LabelListItem {
    protected int ID;

    public BaseEntryListItem() {
      var dragHandle = new VisualElement {
        tooltip = "Drag onto an entry reference to assign it.",
      };
      dragHandle.AddToClassList("editable-item__drag-handle");
      dragHandle.RegisterCallback<PointerDownEvent>(HandlePointerDown);

      var dragIcon = new VisualElement();
      dragIcon.AddToClassList("editable-item__drag-icon");

      Root.Insert(0, dragHandle);
      dragHandle.Add(dragIcon);
    }

    private void HandlePointerDown(PointerDownEvent evt) {
      DragAndDrop.PrepareStartDrag();
      DragAndDrop.paths = new[] { ID.ToString() };
      DragAndDrop.StartDrag("Drag entry");
      evt.StopPropagation();
    }

    protected void SetType(SerializedProperty property) {
      if (property.propertyType != SerializedPropertyType.ManagedReference
        || string.IsNullOrEmpty(property.managedReferenceFullTypename)) {
        Type.text = "";
        return;
      }

      var typeName = property.managedReferenceFullTypename;
      if (TypewriterUtils.TryParseType(typeName, out var type)
        && EntryTypeCache.TryGetDescriptor(type, out var attribute)) {
        Type.text = attribute.Name ?? type.Name;
        Type.style.color = attribute.ParsedColor;
      } else {
        Type.text = property.managedReferenceFullTypename.Split('.').Last();
        Type.style.color = Color.white;
      }
    }

    protected void SetID(SerializedProperty property) {
      ID = property.FindPropertyRelative(nameof(BaseEntry.ID))?.intValue ?? 0;
    }

    public override void BindProperty(SerializedProperty property) {
      base.BindProperty(property);
      SetType(property);
      SetID(property);
    }
  }
}
