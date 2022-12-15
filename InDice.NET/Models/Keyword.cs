﻿namespace InDice.NET.Models;

public class Keyword
{
    public string Index { get; set; } = null!;
    public string OriginalText { get; set; } = null!;
    public string PropertyName { get; set; } = null!;
    public string Match { get; set; } = null!;
    public int LevenshteinDistance { get; set; }
    public double Similarity { get; set; }
}
