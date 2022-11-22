using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests;

public class DefaultExtractorTests
{
    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { "ANDERS", "RUNDGREN" }, "")]
    [InlineData("+Anders Rundgren", new string[] { "ANDERS" }, "Rundgren")]
    [InlineData("+Anders \"Rundgren\"", new string[] { "ANDERS", "RUNDGREN" }, "")]
    [InlineData("+\"Anders Rundgren\"", new string[] { "ANDERSRUNDGREN" }, "")]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { "ANDERSRUNDGREN", "MALMÖ" }, "")]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { "ANDERSRUNDGREN" }, "Malmö")]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { "ANDERSRUNDGREN" }, "-Malmö")]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { "ANDERSRUNDGREN", "MALMÖ" }, "")]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { "ANDERSRUNDGREN" }, "Malmö")]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { "ANDERSRUNDGREN" }, "-Malmö")]
    public void Extracting_explicit_matches_returns_only_explicit_keywords_and_removes_them_in_out_string (string source, string[] keywords, string remainder)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractExplicitMatches(source, out source);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
        Assert.Equal(remainder, source);
    }

    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { }, "+Anders +Rundgren")]
    [InlineData("+Anders Rundgren", new string[] { }, "+Anders Rundgren")]
    [InlineData("+Anders \"Rundgren\"", new string[] { }, "+Anders \"Rundgren\"")]
    [InlineData("+\"Anders Rundgren\"", new string[] { }, "+\"Anders Rundgren\"")]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { }, "\"Anders Rundgren\" +Malmö")]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { }, "\"Anders Rundgren\" Malmö")]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { "MALMÖ" }, "\"Anders Rundgren\"")]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { }, "+\"Anders Rundgren\" +Malmö")]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { }, "+\"Anders Rundgren\" Malmö")]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { "MALMÖ" }, "+\"Anders Rundgren\"")]
    public void Extracting_exclusions_returns_only_exclusion_keywords_and_removes_them_in_out_string(string source, string[] keywords, string remainder)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractExcludedMatches(source, out source);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
        Assert.Equal(remainder, source);
    }

    [Theory]
    [InlineData("+Anders +Rundgren", new string[] { }, "+Anders +Rundgren")]
    [InlineData("+Anders Rundgren", new string[] { "RUNDGREN" }, "+Anders")]
    [InlineData("+Anders \"Rundgren\"", new string[] { }, "+Anders \"Rundgren\"")]
    [InlineData("+\"Anders Rundgren\"", new string[] { }, "+\"Anders Rundgren\"")]
    [InlineData("\"Anders Rundgren\" +Malmö", new string[] { }, "\"Anders Rundgren\" +Malmö")]
    [InlineData("\"Anders Rundgren\" Malmö", new string[] { "MALMÖ" }, "\"Anders Rundgren\"")]
    [InlineData("\"Anders Rundgren\" -Malmö", new string[] { }, "\"Anders Rundgren\" -Malmö")]
    [InlineData("+\"Anders Rundgren\" +Malmö", new string[] { }, "+\"Anders Rundgren\" +Malmö")]
    [InlineData("+\"Anders Rundgren\" Malmö", new string[] { "MALMÖ" }, "+\"Anders Rundgren\"")]
    [InlineData("+\"Anders Rundgren\" -Malmö", new string[] { }, "+\"Anders Rundgren\" -Malmö")]
    public void Extracting_implicit_matches_returns_only_implicit_keywords_and_removes_them_in_out_string(string source, string[] keywords, string remainder)
    {
        // Given
        var extractor = Given_an_extractor();

        // When
        var result = extractor.ExtractImplicitMatches(source, out source);

        // Then
        Assert.Equal(keywords.OrderBy(_ => _), result.OrderBy(_ => _));
        Assert.Equal(remainder, source);
    }

    public static IExtractor Given_an_extractor() =>
        new DefaultExtractor();
}
