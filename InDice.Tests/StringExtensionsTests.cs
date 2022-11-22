using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void Test()
    {
        // Given
        var extractor = new DefaultExtractor();
        var source = "\"Anders Rundgren\" +Malmö -tjosan kan dansa";

        // When
        var result = source.ToKeywordResult(extractor);

        // Then
        var expected = new KeywordResult
        {
            Explicit = new string[] { "ANDERSRUNDGREN", "MALMÖ" },
            Implicit = new string[] { "KAN", "DANSA" },
            Excluded = new string[] { "TJOSAN" }
        };
        Assert.Equal(expected.Explicit, result.Explicit);
        Assert.Equal(expected.Implicit, result.Implicit);
        Assert.Equal(expected.Excluded, result.Excluded);
    }
}
