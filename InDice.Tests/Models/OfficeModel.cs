namespace InDice.Tests.Models;

[InDiceEntity]
public class OfficeModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [InDiceInclude]
    public string Name { get; set; } = string.Empty;

    [InDiceInclude]
    public Manager? Manager { get; set; }
}
