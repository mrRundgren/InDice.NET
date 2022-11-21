using InDice.NET;

namespace InDice.Tests.Models;

public class OfficeModel : IIndexableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [Keyword]
    public string Name { get; set; } = string.Empty;

    [Keyword]
    public Manager? Manager { get; set; }
}
