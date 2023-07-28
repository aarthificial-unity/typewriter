using System;

namespace Aarthificial.Typewriter.Editor.Descriptors {
  [AttributeUsage(AttributeTargets.Class)]
  public class CustomEntryDescriptorAttribute : Attribute {
    public CustomEntryDescriptorAttribute(Type type) {
      Type = type;
    }

    public Type Type { get; }
  }
}
