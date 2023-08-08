using Aarthificial.Typewriter;
using Aarthificial.Typewriter.Entries;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A component that handles dialogue events.
/// </summary>
/// <remarks>
/// In Typewriter, the dialogue is modeled using a chain of events:
/// 1. An interaction triggers an event.
/// 2. The event triggers a line of dialogue.
/// 3. The line of dialogue triggers another event.
/// 4. And so on.
///
/// This component listens to dialogue events, process them accordingly, and
/// then continues the chain by triggering the next event.
/// </remarks>
public class DialogueResponse : MonoBehaviour {
  [SerializeField] private Text _speaker;
  [SerializeField] private Text _text;
  [SerializeField] private float _charsPerSecond = 32;
  [SerializeField] private DialogueSkipButton _skipButton;
  [SerializeField] private DialogueChoiceButton _choiceButtonPrefab;
  [SerializeField] private GameObject _choiceContainer;
  [SerializeField] private GameObject _container;
  [SerializeField] private GameObject _conversations;

  private float _startTime;
  private bool _textCompleted;
  private DialogueEntry _currentEntry;
  private ITypewriterContext _currentContext;
  private bool IsActive => _currentEntry != null;
  private readonly DialogueEntry[] _matches = new DialogueEntry[4];
  private readonly DialogueChoiceButton[]
    _choices = new DialogueChoiceButton[4];

  private void Awake() {
    _container.SetActive(false);
    for (var i = 0; i < _choices.Length; i++) {
      var choice = Instantiate(_choiceButtonPrefab, _choiceContainer.transform);
      choice.gameObject.SetActive(false);
      choice.Index = i;
      choice.Clicked += HandleChoiceClicked;
      _choices[i] = choice;
    }
  }

  private void OnEnable() {
    TypewriterDatabase.Instance.AddListener(HandleTypewriterEvent);
    _skipButton.Clicked += HandleSkipClicked;
  }

  private void OnDisable() {
    TypewriterDatabase.Instance.RemoveListener(HandleTypewriterEvent);
    _skipButton.Clicked -= HandleSkipClicked;
  }

  private void Update() {
    _conversations.SetActive(!IsActive);
    if (!IsActive || _textCompleted) {
      return;
    }

    var elapsed = Time.time - _startTime;
    var duration = _currentEntry.Text.Length
      / (_charsPerSecond * _currentEntry.Speed);
    var progress = Mathf.Clamp01(elapsed / duration);

    if (progress >= 1) {
      Proceed();
    } else {
      var textLength = Mathf.FloorToInt(_currentEntry.Text.Length * progress);

      // WARNING: This is a very naive approach to revealing the text. I used it
      // so that you don't have to install `TextMeshPro` for this example.
      // In a real game, consider using something like `maxVisibleCharacters`:
      // https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/api/TMPro.TMP_Text.html#TMPro_TMP_Text_maxVisibleCharacters
      _text.text = _currentEntry.Text[..textLength];
    }
  }

  private void HandleTypewriterEvent(
    BaseEntry entry,
    ITypewriterContext context
  ) {
    if (!IsActive && entry is DialogueEntry textEntry) {
      _currentEntry = textEntry;
      _currentContext = context;
      Begin();
    }
  }

  private void HandleSkipClicked() {
    if (!IsActive) {
      return;
    }

    if (!_textCompleted) {
      Proceed();
    } else {
      Finish();
    }
  }

  private void HandleChoiceClicked(int index) {
    if (IsActive) {
      Finish(_matches[index]);
    }
  }

  private void Begin() {
    _startTime = Time.time;
    _textCompleted = false;
    _skipButton.Interactable = true;
    _choiceContainer.SetActive(false);
    _container.SetActive(true);
    _skipButton.Text = ">>";
    _speaker.text = _currentEntry.Speaker.DisplayName;
  }

  private void Proceed() {
    _textCompleted = true;
    _text.text = _currentEntry.Text;

    if (_currentEntry.IsChoice) {
      var matches =
        _currentContext.FindMatchingRules(_currentEntry.ID, _matches);
      if (matches > 0) {
        _skipButton.Interactable = false;
        DisplayOptions(matches);
        return;
      }
    }

    var hasMatchingRule = _currentContext.HasMatchingRule(_currentEntry.ID);
    _skipButton.Text = !hasMatchingRule ? "X" : ">";
  }

  private void Finish(DialogueEntry next = null) {
    var entry = _currentEntry;
    var context = _currentContext;

    if (next != null) {
      entry.Apply(context);
      next.Invoke(context);
      entry = next;
    }

    _currentEntry = null;
    _currentContext = null;
    context.Process(entry);

    if (_currentEntry == null) {
      _container.SetActive(false);
    }
  }

  private void DisplayOptions(int count) {
    _choiceContainer.SetActive(true);
    for (var i = 0; i < count; i++) {
      var choice = _choices[i];
      var entry = _matches[i];
      choice.gameObject.SetActive(true);
      choice.Text = entry.Text;
    }

    for (var i = count; i < _choices.Length; i++) {
      _choices[i].gameObject.SetActive(false);
    }
  }
}
