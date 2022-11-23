using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void Keywords_are_extracted_correcctly()
    {
        // Given
        var extractor = new DefaultExtractor();
        var source = "\"Anders Rundgren\" +Malmö -tjosan kan dansa";

        // When
        var result = source.ToKeywordResult(extractor);

        // Then
        var expected = new KeywordResult
        {
            Explicit = new string[] { "MALMÖ" },
            Implicit = new string[] { "ANDERSRUNDGREN", "KAN", "DANSA" },
            Excluded = new string[] { "TJOSAN" }
        };
        Assert.Equal(expected.Explicit, result.Explicit);
        Assert.Equal(expected.Implicit, result.Implicit);
        Assert.Equal(expected.Excluded, result.Excluded);
    }

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
}
