using Aarthificial.Typewriter.Blackboards;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Tools {
  [Serializable]
  public class TypewriterCriteria {
    [SerializeField]
    internal BlackboardCriterion[] List = Array.Empty<BlackboardCriterion>();
    public bool HasValue => List.Length > 0;
  }
}
