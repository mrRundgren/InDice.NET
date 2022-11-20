
using InDice.NET;
namespace InDice;

public class SomeModel : IIndexableEntity
{
    [Keyword]
    public string Firstname { get; set; } = "";
    [Keyword]
    public string Lastname { get; set; } = "";
    [Keyword]
    public string DisplayName => $"{Firstname} {Lastname}";
    [Keyword]
    public string City { get; set; } = "";
    [Keyword]
    public string Zip { get; set; } = "";
    [Keyword]
    public string Street { get; set; } = "";
}
