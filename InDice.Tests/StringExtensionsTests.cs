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
    [InlineData("ANDERS", "Anders", "[Anders]")]
    [InlineData("ANDER", "Anders", "[Ander]s")]
    [InlineData("ANDE", "Anders", "[Ande]rs")]
    [InlineData("AND", "Anders", "[And]ers")]
    [InlineData("NDRS", "Anders", "[Anders]")]
    [InlineData("NDR", "Anders", "[Ander]s")]
    [InlineData("ND", "Anders", "[And]ers")]
    [InlineData("N", "Anders", "[An]ders")]
    [InlineData("VRLD", "Hej världen", "Hej [värld]en")]
    [InlineData("TST", "This is a very good test", "This is a very good [test]")]
    [InlineData("ONE", "One, two, three", "[One], two, three")]
    [InlineData("TWO", "One, two, three", "One, [two], three")]
    [InlineData("THREE", "One, two, three", "One, two, [three]")]
    public void Index_can_be_matched_to_source_string(string index, string source, string expected)
    {
        // When
        var result = source.ToMatchedString(index);

        // Then
        Assert.Equal(expected, result);
    }

    public static IEncoder Given_an_encoder() =>
        new DefaultEncoder();
}
