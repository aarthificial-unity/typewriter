using Aarthificial.Typewriter.References;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Blackboards {
  [Serializable]
  public struct BlackboardCriterion {
    public enum OperationType {
      [InspectorName(" = ")] Equal,
      [InspectorName(" ~ ")] EqualReference,
      [InspectorName(" ≥ ")] GreaterEqual,
      [InspectorName(" ≤ ")] LessEqual,
      [InspectorName("[...]")] ClosedInterval,
    }

    public EntryReference FactReference;
    public int Min;
    public int Max;
    public OperationType Operation;
  }
}
