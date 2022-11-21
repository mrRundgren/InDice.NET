using InDice.NET;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace InDice.Tests.Models;

[InDiceEntity]
public class OfficeModel : IIndexableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [InDiceGenerate]
    public string Name { get; set; } = string.Empty;

    [InDiceGenerate]
    public Manager? Manager { get; set; }
}
