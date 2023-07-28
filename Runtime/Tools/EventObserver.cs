using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Common;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Aarthificial.Typewriter.Tools {
  public class EventObserver : MonoBehaviour {
    [SerializeField] private EventData[] _listeners;

    private void OnEnable() {
      for (var index = 0; index < _listeners.Length; index++) {
        TypewriterDatabase.Instance.AddListener(
          _listeners[index].EventReference,
          _listeners[index].Invoke
        );
      }
    }

    private void OnDisable() {
      for (var index = 0; index < _listeners.Length; index++) {
        TypewriterDatabase.Instance.RemoveListener(
          _listeners[index].EventReference,
          _listeners[index].Invoke
        );
      }
    }

    [Serializable]
    private struct EventData {
      [EntryFilter(Type = EntryType.Event)]
      public EntryReference EventReference;
      [SerializeField] private bool _once;
      [SerializeField] private UnityEvent _unityEvent;
      private bool _wasDispatched;

      public void Invoke(BaseEntry entry, ITypewriterContext context) {
        if (_once && _wasDispatched) {
          return;
        }

        _wasDispatched = true;
        _unityEvent.Invoke();
      }
    }
  }
}
