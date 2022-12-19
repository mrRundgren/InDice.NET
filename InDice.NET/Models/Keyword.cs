namespace InDice.NET.Models;

public class Keyword
{
    public string Index { get; set; } = null!;
    public int LevenshteinDistance { get; init; }
    public double Similarity { get; init; }
}
