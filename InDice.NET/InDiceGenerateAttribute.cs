using System.Reflection;

namespace InDice.NET;

[AttributeUsage(AttributeTargets.Property)]
public class InDiceGenerateAttribute : Attribute {
    public InDiceGenerateAttribute() { }
}