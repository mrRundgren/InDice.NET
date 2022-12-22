﻿using InDice.NET;

namespace InDice.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("This is \"a string\" to split", new string[] { "This", "is", "a string", "to", "split" })]
    [InlineData("This is +\"a string\" to split", new string[] { "This", "is", "+a string", "to", "split" })]
    [InlineData("This is -\"a string\" to split", new string[] { "This", "is", "-a string", "to", "split" })]
    [InlineData("This is -\"a string to split", new string[] { "This", "is", "-a string to split" })]
    [InlineData("", new string[] { })]
    public void Splitting_a_string_retains_quoted_strings(string source, string[] expected)
    {
        // When
        var result = source.QualifiedSplit(' ', '"');

        // Thenb
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { "ANDERS", "RUNDGREN" })]
    [InlineData("+Anders Rundgren", new string[] { "ANDERS" })]
    [InlineData("+Anders \"Rundgren\"", new string[] { "ANDERS" })]
    [InlineData("+\"Anders Rundgren\"", new string[] { "ANDERSRUNDGREN" })]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { "MALMÖ" })]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { })]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { })]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { "ANDERSRUNDGREN", "MALMÖ" })]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { "ANDERSRUNDGREN" })]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { "ANDERSRUNDGREN" })]
    public void Extracting_explicit_matches_returns_only_explicit_keywords(string source, string[] keywords)
    {
        // Given
        var encoder = Given_an_encoder();

        // When
        var result = source.ExtractExplicits(encoder);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
    }

    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { })]
    [InlineData("+Anders Rundgren", new string[] { "RUNDGREN" })]
    [InlineData("+Anders \"Rundgren är kul\"", new string[] { "RUNDGRENÄRKUL" })]
    [InlineData("+\"Anders Rundgren\"", new string[] { })]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { "ANDERSRUNDGREN" })]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { "ANDERSRUNDGREN", "MALMÖ" })]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { "ANDERSRUNDGREN" })]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { })]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { "MALMÖ" })]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { })]
    public void Extracting_implicit_matches_returns_only_implicit_keywords(string source, string[] expected)
    {
        // Given
        var encoder = Given_an_encoder();

        // When
        var result = source.ExtractImplicits(encoder);

        // Then
        Assert.Equal(expected.OrderBy(_ => _), result.OrderBy(_ => _));
    }

    [Theory]
    [InlineData("+Anders +Rundgren -\"Detta kommer inte med\"", new string[] { "DETTAKOMMERINTEMED" })]
    [InlineData("+Anders Rundgren", new string[] { })]
    [InlineData("+Anders \"Rundgren\"", new string[] { })]
    [InlineData("+\"Anders Rundgren\"", new string[] { })]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { })]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { })]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { "MALMÖ" })]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { })]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { })]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { "MALMÖ" })]
    public void Extracting_exclusions_returns_only_exclusion_keyword(string source, string[] keywords)
    {
        // Given
        var encoder = Given_an_encoder();
        // When
        var result = source.ExtractExclusions(encoder);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
    }

    [Fact]
    public void Extracting_explicits_without_encoder_extracts_correctly()
    {
        // Given
        var source = "+explicit implicit -excluded";
        var expected = new string[] { "explicit" };

        // When
        var result = source.ExtractExplicits();

        // Then
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Extracting_implicits_without_encoder_extracts_correctly()
    {
        // Given
        var source = "+explicit implicit -excluded";
        var expected = new string[] { "implicit" };

        // When
        var result = source.ExtractImplicits();

        // Then
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Extracting_Exclusions_without_encoder_extracts_correctly()
    {
        // Given
        var source = "+explicit implicit -excluded";
        var expected = new string[] { "excluded" };

        // When
        var result = source.ExtractExclusions();

        // Then
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("anders rundgren", "\"anders rundgren\"")]
    [InlineData("anders rundgren +human", "anders rundgren +human")]
    public void Absolute_string_is_transformed_correctly(string source, string expected)
    {
        // When
        var result = source.ToAbsoluteString();

        // Then
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("anders", "Anders", 0)]
    [InlineData("Good will hunting", "hunting", 8)]
    [InlineData("Enter the matrix", "the", 11)]
    [InlineData("", "the", 3)]
    [InlineData("the", "", 3)]
    [InlineData("", "", 0)]
    [InlineData("the", "the", 0)]
    [InlineData("a string", "", 7)]
    [InlineData("", "a string", 7)]
    public void Levenshtein_distance_can_be_calculated(string source, string target, int expected)
    {
        // When
        var result = StringExtensions.ToLevenshteinDistance(source, target);

        // Then
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("anders", "Anders", 1)]
    [InlineData("good will hunting", "HuNtinG", 0.47)]
    [InlineData("enter the matrix", "the", 0.21)]
    [InlineData("", "the", 0)]
    [InlineData("the", "", 0)]
    [InlineData("", "", 1)]
    [InlineData("The", "the", 1)]
    public void Similarity_can_be_calculated(string source, string target, double expected)
    {
        // When
        var result = StringExtensions.ToSimilarity(source, target);

        // Then
        Assert.Equal(expected, result);
    }

    [Theory]
    //[InlineData("a aven the avengers \"the avengers\"", "The Avengers", "[The Avengers]")]
    [InlineData("+tHE onLY", "the one and only olony", "[the] one and [only] olony")]
    [InlineData("+One aNd", "the one and only olony", "the [one] [and] only olony")]
    [InlineData("+\"the avenger\"", "The Avengers", "[The Avenger]s")]
    [InlineData("one two", "one two three two", "[one] [two] three [two]")]
    [InlineData("one two", "one two three two helixcondrone", "[one] [two] three [two] helixcondrone")]
    //[InlineData("\"one two\" two", "one two three two helixcondrone", "[one two] three [two] helixcondrone")]
    [InlineData("+\"joss whedon\"", "Joss Whedon (screenplay), Zak Penn (story), Joss Whedon (story)", "[Joss Whedon] (screenplay), Zak Penn (story), [Joss Whedon] (story)")]
    [InlineData("+test", "this has no words that begin withtest", "this has no words that begin withtest")]
    public void Can_apply_matches_from_search_string_to_string(string search, string source, string expected)
    {
        // When
        var result = StringExtensions.FindMatches(source, search);

        // Then
        Assert.Equal(expected, result);
    }

    public static IEncoder Given_an_encoder() =>
        new DefaultEncoder();
}
