using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Lists {
  public class ExpandableListView : VisualElement {
    public readonly Foldout Foldout;
    public readonly SimpleList List;

    public ExpandableListView() {
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Lists/ExpandableListView");
      visualTree.CloneTree(this);

      Foldout = this.Q<Foldout>("foldout");
      List = this.Q<SimpleList>("list");
      List.Synchronized += Synchronize;
      List.SelectionChanged += HandleSelectionChanged;
      Synchronize();

      Foldout.RegisterValueChangedCallback(
        evt => {
          if (!evt.newValue) {
            List.ClearSelection();
          }
        }
      );
    }

    public string Text {
      get => Foldout.text;
      set => Foldout.text = value;
    }

    private void HandleSelectionChanged(List<SimpleList.ItemData> selection) {
      if (selection.Count > 0) {
        Foldout.value = true;
      }
    }

    private void Synchronize() {
      style.display =
        List.Source.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void BindProperty(SerializedProperty property) {
      List.BindProperty(property);
    }

    public new class
      UxmlFactory : UxmlFactory<ExpandableListView, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits {
      private readonly UxmlStringAttributeDescription _text =
        new() { name = "text" };

      public override void Init(
        VisualElement ve,
        IUxmlAttributes bag,
        CreationContext cc
      ) {
        base.Init(ve, bag, cc);
        ((ExpandableListView)ve).Text = _text.GetValueFromBag(bag, cc);
      }
    }
  }
}
