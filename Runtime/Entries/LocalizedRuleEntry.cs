#if UNITY_LOCALIZATION
using System;
using UnityEngine.Localization;

namespace Aarthificial.Typewriter.Entries {
  [Serializable]
  public class LocalizedRuleEntry : RuleEntry {
    public LocalizedString Text = new();

    public override string ToString() {
      return $"{Text} ({ID})";
    }
  }
}
#endif
