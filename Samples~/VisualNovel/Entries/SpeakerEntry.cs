using Aarthificial.Typewriter.Entries;
using System;

/// <summary>
/// An fact representing a speaker.
/// </summary>
/// <remarks>
/// Typewriter entries can be extended to store additional information specific
/// to your game.
/// </remarks>
[Serializable]
public class SpeakerEntry : FactEntry {
  /// <summary>
  /// The speaker's name.
  /// </summary>
  public string DisplayName;
}
