namespace InDice.Tests.Models
{
    [InDiceEntity]
    public class FieldModel : IIndexableEntity
    {
        [InDiceGenerate] public string Name { get; set; } = null!;
    }
}
