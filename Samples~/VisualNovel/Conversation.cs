using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Tools;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a conversation.
/// </summary>
/// <remarks>
/// A conversation can be started by pressing the corresponding button. Doing so
/// will trigger the Typewriter event configured in the inspector. Each
/// conversation has its own context, which is passed along with the event.
/// </remarks>
public class Conversation : MonoBehaviour {
  [SerializeField] private Button _button;
  [SerializeField] private EntryReference _event;

  private readonly Context _context = new();
  private TypewriterWatcher _watcher;

  private void OnEnable() {
    _button.onClick.AddListener(HandleClicked);
  }

  private void OnDisable() {
    _button.onClick.RemoveListener(HandleClicked);
  }

  private void Update() {
    if (_watcher.ShouldUpdate()) {
      _button.interactable = _context.WouldInvoke(_event);
    }
  }

  private void HandleClicked() {
    _context.TryInvoke(_event);
  }
}
