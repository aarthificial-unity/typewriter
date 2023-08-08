using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A rule representing a line of dialogue.
/// </summary>
/// <remarks>
/// Typewriter entries can be extended to store additional information specific
/// to your game.
/// </remarks>
[Serializable]
public class DialogueEntry : RuleEntry {
  /// <summary>
  /// The speed at which the text is revealed.
  /// </summary>
  [Range(0.25f, 2)] public float Speed = 1f;

  /// <summary>
  /// Whether this line should end with a choice.
  /// </summary>
  public bool IsChoice;

  /// <summary>
  /// The dialogue line.
  /// </summary>
  [TextArea] public string Text;

  /// <summary>
  /// A helper method for resolving the speaker reference.
  /// </summary>
  public SpeakerEntry Speaker {
    get {
      var speaker = _speaker.GetEntry<SpeakerEntry>();
      Assert.IsNotNull(
        speaker,
        $"Invalid speaker ID ({_speaker.ID}) required by \"{this}\""
      );
      return speaker;
    }
  }

  /// <summary>
  /// The speaker saying this line.
  /// </summary>
  /// <remarks>
  /// We can use the <see cref="EntryFilterAttribute"/> to restrict the entries
  /// we can reference to a specific type.
  /// </remarks>
  [EntryFilter(BaseType = typeof(SpeakerEntry))]
  [SerializeField]
  private EntryReference _speaker;
}
