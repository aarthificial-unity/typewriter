using Aarthificial.Typewriter.Common;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Aarthificial.Typewriter.Editor.Build {
  internal class TypewriterBuildPlayer : IPreprocessBuildWithReport,
    IPostprocessBuildWithReport {
    private TypewriterDatabase _database;
    private bool _removeFromPreloadedAssets;

    public void OnPostprocessBuild(BuildReport report) {
      if (_database == null || !_removeFromPreloadedAssets) {
        return;
      }

      var preloadedAssets = PlayerSettings.GetPreloadedAssets();
      ArrayUtility.Remove(ref preloadedAssets, _database);
      PlayerSettings.SetPreloadedAssets(preloadedAssets);
      _database = null;
    }

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report) {
      _database = TypewriterDatabase.Instance;
      if (_database == null) {
        return;
      }

      var preloadedAssets = PlayerSettings.GetPreloadedAssets();
      if (!preloadedAssets.Contains(_database)) {
        ArrayUtility.Add(ref preloadedAssets, _database);
        PlayerSettings.SetPreloadedAssets(preloadedAssets);
        _removeFromPreloadedAssets = true;
      }
    }
  }
}
