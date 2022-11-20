using InDice.NET;

namespace InDice.Tests.Models;

public class PersonModel : IIndexableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [Keyword]
    public string Firstname { get; set; } = string.Empty;
    [Keyword]
    public string Lastname { get; set; } = string.Empty;
    [Keyword]
    public int HiringNo { get; set; }
    [Keyword]
    public string DisplayName
    {
        get => $"{Firstname} {Lastname}";
    }
    [Keyword]
    public OfficeModel? Office { get; set; } = null;
}
