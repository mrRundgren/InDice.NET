﻿namespace InDice.Tests.Models;

[InDiceEntity]
public class OfficeModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [InDiceGenerate]
    public string Name { get; set; } = string.Empty;

    [InDiceGenerate]
    public Manager? Manager { get; set; }
}
