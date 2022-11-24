namespace InDice.Tests.Models;

[InDiceEntity]
public class Manager
{
    [InDiceInclude]
    public string Name { get; set; } = null!;
}
