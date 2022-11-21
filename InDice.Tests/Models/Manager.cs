namespace InDice.Tests.Models;

[InDiceEntity]
public class Manager : IIndexableEntity
{
    [InDiceGenerate]
    public string Name { get; set; } = null!;
}
