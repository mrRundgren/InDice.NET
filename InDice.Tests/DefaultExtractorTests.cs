using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests;

public class DefaultExtractorTests
{
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
    public void Extracting_explicit_matches_returns_only_explicit_keywords_and_removes_them_in_out_string(string source, string[] keywords)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractExplicits(source);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
    }

    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { })]
    [InlineData("+Anders Rundgren", new string[] { "RUNDGREN" })]
    [InlineData("+Anders \"Rundgren är kul\"", new string[] { "RUNDGRENÄRKUL" })]
    [InlineData("+\"Anders Rundgren\"", new string[] { })]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] {"ANDERSRUNDGREN" })]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { "ANDERSRUNDGREN", "MALMÖ" })]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { "ANDERSRUNDGREN" })]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { })]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { "MALMÖ" })]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { })]
    public void Extracting_implicit_matches_returns_only_implicit_keywords_and_removes_them_in_out_string(string source, string[] expected)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractImplicits(source);

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
    public void Extracting_exclusions_returns_only_exclusion_keywords_and_removes_them_in_out_string(string source, string[] keywords)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractExclusions(source);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
    }

    [Theory]
    [InlineData("Anders", new string[] { "NDRS" })]
    public void An_extractor_with_unsafe_chars_encodes_correctly(string source, string[] expected)
    {
        // Given 
        var (extractor, _) = Given_an_extractor_and_unsafe_chars();

        // When
        var result = extractor.ExtractImplicits(source);

        // Then
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("The Anders", new string[] { "NDRS" })]
    public void Excluded_words_are_not_included(string source, string[] expected)
    {
        // Given 
        var extractor = Given_an_extractor_and_excluded_words();

        // When
        var result = extractor.ExtractImplicits(source);

        // Then
        Assert.Equal(expected, result);
    }

    public static IExtractor Given_an_extractor() =>
        new DefaultExtractor();

    public static (IExtractor extractor, string unsafeChars) Given_an_extractor_and_unsafe_chars() =>
        (new DefaultExtractor("AOUÅEIYÄÖ"), "AOUÅEIYÄÖ");

    public static IExtractor Given_an_extractor_and_excluded_words() =>
        new DefaultExtractor("AOUÅEIYÄÖ", new string[] {"The"});
}
