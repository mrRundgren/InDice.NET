namespace InDice.Tests.Models;

[InDiceEntity]
public class PersonModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [InDiceInclude]
    public string Firstname { get; set; } = string.Empty;
    [InDiceInclude]
    public string Lastname { get; set; } = string.Empty;
    [InDiceInclude]
    public int HiringNo { get; set; }
    [InDiceInclude]
    public string DisplayName
    {
        get => $"{Firstname} {Lastname}";
    }
    [InDiceInclude]
    public OfficeModel? Office { get; set; } = null;

    [InDiceInclude]
    public List<FieldModel> Fields {get; set;} = new ();
}
