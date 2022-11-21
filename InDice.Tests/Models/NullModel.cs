using InDice.NET;

namespace InDice.Tests.Models;

[InDiceEntity]
public class NullModel
{
    public string? Name { get; set; } = null;
}
