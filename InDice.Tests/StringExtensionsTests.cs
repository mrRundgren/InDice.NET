using InDice.NET;

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

        // Then
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

    public static IEncoder Given_an_encoder() =>
        new DefaultEncoder();
}
