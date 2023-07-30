using Aarthificial.Typewriter.Blackboards;

namespace Aarthificial.Typewriter.Common {
  public interface ITypewriterContext {
    public bool TryGetBlackboard(int scope, out IBlackboard blackboard);
  }
}
