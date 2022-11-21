namespace InDice.Tests.Models
{
    [InDiceEntity]
    public class FieldModel
    {
        [InDiceGenerate] public string Name { get; set; } = null!;
    }
}
