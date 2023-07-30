using Aarthificial.Typewriter.Blackboards;

namespace Aarthificial.Typewriter {
  public interface ITypewriterContext {
    public bool TryGetBlackboard(int scope, out IBlackboard blackboard);
  }
}
