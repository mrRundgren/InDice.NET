namespace InDice.NET;

[AttributeUsage(AttributeTargets.Property)]
public class InDiceIncludeAttribute : Attribute {

    public InDiceGenerateMode Mode { get; set; } = InDiceGenerateMode.Default;
    public bool IncludeReverseOrder { get; set; } = false;
    public InDiceIncludeAttribute(InDiceGenerateMode mode = InDiceGenerateMode.Default, bool includeReverseOrder = false) 
    {
        Mode = mode;
        IncludeReverseOrder = mode == InDiceGenerateMode.SplitOnWords && includeReverseOrder;
    }
}