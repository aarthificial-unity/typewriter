using Aarthificial.Typewriter.Blackboards;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Tools {
  [Serializable]
  public class TypewriterModification {
    [SerializeField]
    internal BlackboardModification[] List =
      Array.Empty<BlackboardModification>();
    public bool HasValue => List.Length > 0;
  }
}
