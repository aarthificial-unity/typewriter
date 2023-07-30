namespace Aarthificial.Typewriter.Tools {
  public struct TypewriterWatcher {
    private int _lastUpdate;

    public bool ShouldUpdate() {
      return TypewriterDatabase.Instance.HasChangedSince(ref _lastUpdate);
    }
  }
}
