namespace InDice.Tests.Models;

[InDiceEntity]
public class FieldModel
{
    [InDiceInclude]
    public string Name { get; set; } = null!;
}
