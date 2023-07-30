using Aarthificial.Typewriter.Editor.Layout.Inspector;
using Aarthificial.Typewriter.Entries;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  /// <summary>Describes an entry to make it recognizable by the Typewriter editor.</summary>
  public class EntryDescriptor {
    public Type RealType { get; internal set; }

    /// <summary>Display name. Defaults to the class name.</summary>
    public virtual string Name => RealType.Name;

    /// <summary>Entry type.</summary>
    public virtual EntryVariant Variant => EntryVariant.Rule;

    /// <summary>Display color.</summary>
    public virtual string Color => "#ffffff";

    /// <summary>
    ///   Whether or not the entry is optional. Optional entries are shown only
    ///   if no other entry is available.
    /// </summary>
    public virtual bool Optional => false;

    /// <summary>
    ///   Whether or not the entry should have a customization panel. Criteria,
    ///   Modifications, etc.
    /// </summary>
    public virtual bool HasCustomization => true;

    /// <summary>Whether or not the entry should have a navigation panel.</summary>
    public virtual bool HasNavigation => true;

    public Color ParsedColor {
      get {
        ColorUtility.TryParseHtmlString(Color, out var color);
        return color;
      }
    }

    /// <summary>Construct the "next" menu of <see cref="Navigator" />.</summary>
    /// <param name="entry">The entry for which the menu should be constructed.</param>
    /// <param name="names">
    ///   An empty list that should be filled with names of next
    ///   entries.
    /// </param>
    /// <param name="ids">An empty list that should be filled with ids of next entries.</param>
    /// <param name="current">The index of the currently select option.</param>
    public virtual void CreateNextMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) { }

    /// <summary>Construct the "branches" menu of <see cref="Navigator" />.</summary>
    /// <param name="entry">The entry for which the menu should be constructed.</param>
    /// <param name="names">
    ///   An empty list that should be filled with names of branch
    ///   entries.
    /// </param>
    /// <param name="ids">
    ///   An empty list that should be filled with ids of branch
    ///   entries.
    /// </param>
    /// <param name="current">The index of the currently select option.</param>
    public virtual void CreateAlternativeMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) { }

    /// <summary>Construct the "previous" menu of <see cref="Navigator" />.</summary>
    /// <param name="entry">The entry for which the menu should be constructed.</param>
    /// <param name="names">
    ///   An empty list that should be filled with names of previous
    ///   entries.
    /// </param>
    /// <param name="ids">
    ///   An empty list that should be filled with ids of previous
    ///   entries.
    /// </param>
    /// <param name="current">The index of the currently select option.</param>
    public virtual void CreatePreviousMenu(
      BaseEntry entry,
      List<string> names,
      List<int> ids,
      ref int current
    ) { }

    /// <summary>Do something to a newly created entry.</summary>
    /// <param name="entry">The newly created entry.</param>
    /// <param name="table">The table to which the new entry belongs.</param>
    public virtual void HandleEntryCreated(
      BaseEntry entry,
      DatabaseTable table
    ) { }
  }
}
