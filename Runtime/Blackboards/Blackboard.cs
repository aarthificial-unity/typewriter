using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Typewriter.Blackboards {
  [Serializable]
  public class Blackboard : ISerializationCallbackReceiver, IBlackboard {
    [SerializeField] internal List<Pair> List = new();
    private Dictionary<int, int> _lookup = new();

    public void Set(int key, int value) {
      if (_lookup.TryGetValue(key, out var index)) {
        var pair = List[index];
        pair.Value = value;
        List[index] = pair;
      } else {
        index = List.Count;
        List.Add(
          new Pair {
            Key = key,
            Value = value,
          }
        );
        _lookup.Add(key, index);
      }
    }

    public void Add(int key, int value) {
      if (_lookup.TryGetValue(key, out var index)) {
        var pair = List[index];
        pair.Value += value;
        List[index] = pair;
      } else {
        index = List.Count;
        List.Add(
          new Pair {
            Key = key,
            Value = value,
          }
        );
        _lookup.Add(key, index);
      }
    }

    public int Get(int key) {
      return _lookup.TryGetValue(key, out var index) ? List[index].Value : 0;
    }

    public bool Test(BlackboardCriterion criterion) {
      var value = Get(criterion.FactReference.ID);

      return value >= criterion.Min && value <= criterion.Max;
    }

    public void Modify(BlackboardModification modification) {
      switch (modification.Operation) {
        case BlackboardModification.OperationType.SetReference:
        case BlackboardModification.OperationType.Set:
          Set(modification.FactReference.ID, modification.Value);
          break;
        case BlackboardModification.OperationType.Add:
          Add(modification.FactReference.ID, modification.Value);
          break;
      }
    }

    public void Clear() {
      _lookup.Clear();
      List.Clear();
    }

    public IBlackboard Clone() {
      return new Blackboard {
        List = new List<Pair>(List),
        _lookup = new Dictionary<int, int>(_lookup),
      };
    }

    public IBlackboard CloneWith(IBlackboard other) {
      var clone = Clone();
      clone.MergeWith(other);

      return clone;
    }

    public void MergeWith(IBlackboard other) {
      foreach (var pair in other) {
        Set(pair.Key, pair.Value);
      }
    }

    public void ReplaceWith(IBlackboard other) {
      List = new List<Pair>(other);
      OnAfterDeserialize();
    }

    public IEnumerator<Pair> GetEnumerator() {
      return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize() {
      _lookup.Clear();
      for (var i = 0; i < List.Count; i++) {
        _lookup[List[i].Key] = i;
      }
    }

    [Serializable]
    public struct Pair {
      [EntryFilter(PreferredVariant = EntryVariant.Fact)]
      public EntryReference Key;
      public int Value;
    }
  }
}
