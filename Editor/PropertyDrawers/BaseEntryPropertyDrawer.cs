using Aarthificial.Typewriter.Editor.Descriptors;
using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Layout.Inspector;
using Aarthificial.Typewriter.Entries;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.PropertyDrawers {
  [CustomPropertyDrawer(typeof(BaseEntry), true)]
  public class BaseEntryPropertyDrawer : PropertyDrawer {
    protected virtual IEnumerable<string> GetHandledFields() {
      return new[] {
        nameof(BaseEntry.ID),
        nameof(BaseEntry.Key),
        nameof(BaseEntry.Modifications),
        nameof(BaseEntry.Criteria),
        nameof(RuleEntry.OnApply),
        nameof(RuleEntry.OnInvoke),
      };
    }

    private IEnumerable<SerializedProperty> GetChildren(
      SerializedProperty property
    ) {
      var iterator = property.Copy();
      var end = iterator.GetEndProperty();

      if (!iterator.NextVisible(true)) {
        yield break;
      }

      var omittedFields = GetHandledFields().ToList();
      do {
        if (SerializedProperty.EqualContents(iterator, end)) {
          yield break;
        }

        if (omittedFields.Contains(iterator.name)) {
          continue;
        }

        yield return iterator;
      } while (iterator.NextVisible(false));
    }

    public override VisualElement CreatePropertyGUI(
      SerializedProperty property
    ) {
      var root = new VisualElement();
      var visualTree =
        Resources.Load<VisualTreeAsset>("UXML/Layout/InspectorView");
      visualTree.CloneTree(root);

      var container = root.Q<VisualElement>("content");
      root.style.flexGrow = 1;

      EntryTypeCache.TryGetDescriptor(property.FindEntry(), out var descriptor);

      if (descriptor?.HasNavigation ?? true) {
        var navigator = new Navigator();
        navigator.BindProperty(property);
        container.Add(navigator);
      }

      PopulateContent(container, property);

      if (descriptor?.HasCustomization ?? false) {
        var blackboard = new Customization();
        blackboard.BindProperty(property);
        root.Add(blackboard);
      }

      return root;
    }

    protected virtual void PopulateContent(
      VisualElement root,
      SerializedProperty property
    ) {
      foreach (var child in GetChildren(property)) {
        root.Add(new PropertyField(child));
      }
    }
  }
}
