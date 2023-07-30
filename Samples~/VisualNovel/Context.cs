using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Common;
using System;

/// <summary>
/// A sample implementation of <see cref="ITypewriterContext"/>.
/// </summary>
/// <remarks>
/// In this example, the context is made up of two blackboards. The global
/// blackboard is shared between all contexts, while the context blackboard is
/// local to the given conversation.
/// </remarks>
[Serializable]
public class Context : ITypewriterContext {
  private const int _globalScope = 1388552;
  private const int _contextScope = 1388553;

  private static Blackboard _global = new();
  private Blackboard _context = new();

  public bool TryGetBlackboard(int scope, out IBlackboard blackboard) {
    switch (scope) {
      case _globalScope:
        blackboard = _global;
        return true;
      case _contextScope:
        blackboard = _context;
        return true;
      default:
        blackboard = default;
        return false;
    }
  }
}
