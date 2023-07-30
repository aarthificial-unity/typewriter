using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI button for picking a dialogue choice.
/// </summary>
public class DialogueChoiceButton : MonoBehaviour {
  [SerializeField] private Text _text;
  [SerializeField] private Button _button;
  [NonSerialized] public int Index;

  public event Action<int> Clicked;
  public string Text {
    set => _text.text = value;
  }

  private void OnEnable() {
    _button.onClick.AddListener(HandleClicked);
  }

  private void OnDisable() {
    _button.onClick.RemoveListener(HandleClicked);
  }

  private void HandleClicked() {
    Clicked?.Invoke(Index);
  }
}
