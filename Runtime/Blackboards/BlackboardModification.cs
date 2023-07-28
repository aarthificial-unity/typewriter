using Aarthificial.Typewriter.References;
using System;
using UnityEngine;

namespace Aarthificial.Typewriter.Blackboards {
  [Serializable]
  public struct BlackboardModification {
    public enum OperationType {
      [InspectorName(" = ")] Set,
      [InspectorName(" ~ ")] SetReference,
      [InspectorName(" + ")] Add,
    }

    public EntryReference FactReference;
    public int Value;
    [SerializeField] public OperationType Operation;
  }
}
