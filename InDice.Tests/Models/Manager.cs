namespace InDice.Tests.Models;

[InDiceEntity]
public class Manager
{
    [InDiceGenerate]
    public string Name { get; set; } = null!;
}
