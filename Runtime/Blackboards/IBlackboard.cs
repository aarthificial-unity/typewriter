using System.Collections.Generic;

namespace Aarthificial.Typewriter.Blackboards {
  public interface IBlackboard : IEnumerable<Blackboard.Pair> {
    void Set(int key, int value);
    void Add(int key, int value);
    int Get(int key);
    bool Test(BlackboardCriterion criterion);
    void Modify(BlackboardModification modification);
    void Clear();

    /// <summary>Create new blackboard containing the same facts as this instance.</summary>
    /// <returns>Newly created blackboard instance.</returns>
    IBlackboard Clone();

    /// <summary>
    ///   Create new blackboard with facts from both this instance and another.
    ///   blackboard.
    /// </summary>
    /// <param name="other">Source of additional facts</param>
    /// <returns>Newly created blackboard instance.</returns>
    IBlackboard CloneWith(IBlackboard other);

    /// <summary>Add facts from another blackboard to this instance.</summary>
    /// <param name="other">Source of additional facts.</param>
    void MergeWith(IBlackboard other);

    /// <summary>Replace contents of this instance with facts from another blackboard.</summary>
    /// <param name="other">Source of new facts.</param>
    void ReplaceWith(IBlackboard other);
  }
}
