using InDice.NET;

namespace InDice.Tests.Models;

public class Manager : IIndexableEntity
{
    [Keyword]
    public string Name { get; set; } = null!;
}
