using UnityEditor;
using Packages.Rider.Editor.ProjectGeneration;

public static class Solution {
  public static void Sync() {
    AssetDatabase.Refresh();
    new ProjectGeneration().Sync();
  }
}
