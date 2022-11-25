namespace InDice.NET;

[AttributeUsage(AttributeTargets.Property)]
public class InDiceIncludeAttribute : Attribute {

    public InDiceGenerateMode Mode { get; set; } = InDiceGenerateMode.Default;
    public InDiceIncludeAttribute() { }

    public InDiceIncludeAttribute(InDiceGenerateMode mode) {
        Mode = mode;
    }
}