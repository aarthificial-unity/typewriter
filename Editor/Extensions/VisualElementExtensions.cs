using System.Text;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor.Extensions {
  public static class VisualElementExtensions {
    public static string GetViewDataKey(this VisualElement ve) {
      var builder = new StringBuilder();
      var current = ve;

      while (current.parent != null) {
        if (current.viewDataKey == "rootVisualContainer") {
          break;
        }

        builder.Append(current.viewDataKey);
        current = current.parent;
      }

      return builder.ToString();
    }
  }
}
