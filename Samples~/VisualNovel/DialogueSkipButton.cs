using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A UI button for skipping and progressing the dialogue.
/// </summary>
public class DialogueSkipButton : MonoBehaviour {
  [SerializeField] private Text _text;
  [SerializeField] private Button _button;

  public event UnityAction Clicked {
    add => _button.onClick.AddListener(value);
    remove => _button.onClick.RemoveListener(value);
  }

  public string Text {
    set => _text.text = value;
  }

  public bool Interactable {
    set => _button.interactable = value;
  }
}
