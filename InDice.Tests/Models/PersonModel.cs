using InDice.NET;

namespace InDice.Tests.Models;

[InDiceEntity]
public class PersonModel : IIndexableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [InDiceGenerate]
    public string Firstname { get; set; } = string.Empty;
    [InDiceGenerate]
    public string Lastname { get; set; } = string.Empty;
    [InDiceGenerate]
    public int HiringNo { get; set; }
    [InDiceGenerate]
    public string DisplayName
    {
        get => $"{Firstname} {Lastname}";
    }
    [InDiceGenerate]
    public OfficeModel? Office { get; set; } = null;

    [InDiceGenerate]
    public List<FieldModel> Fields {get; set;} = new ();
}
