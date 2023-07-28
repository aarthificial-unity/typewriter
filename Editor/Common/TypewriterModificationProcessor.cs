using Aarthificial.Typewriter.Common;
using UnityEditor;

namespace Aarthificial.Typewriter.Editor.Common {
  public class TypewriterModificationProcessor : AssetModificationProcessor {
    private static AssetDeleteResult OnWillDeleteAsset(
      string assetPath,
      RemoveAssetOptions options
    ) {
      if (AssetDatabase.LoadAssetAtPath<TypewriterDatabase>(assetPath)
        != null) {
        TypewriterUtils.Events.OnDatabaseRemoved();
      }

      return AssetDeleteResult.DidNotDelete;
    }
  }
}
