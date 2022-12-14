namespace InDice.Tests;

public class DefaultEncoderTests
{
    [Theory]
    [InlineData("Malm?", "MLM")]
    [InlineData("Anders", "NDRS")]
    [InlineData("Rundgren", "RNDGRN")]
    [InlineData("Anders Rundgren", "NDRSRNDGRN")]
    public void Encoding_a_keyword_gives_the_expected_result(string keyword, string expected)
    {
        // Given
        var encoder = Given_an_encoder();

        // When
        var result = encoder.Encode(keyword);

        //Then
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Malm?", "MLM", "A?")]
    [InlineData("Anders", "AOU?EIY??", "NDRS")]
    [InlineData("Rundgren", "AOU?EIY??", "RNDGRN")]
    [InlineData("Anders Rundgren", "", "ANDERSRUNDGREN")]
    [InlineData("", "", "")]
    public void Unsafe_characters_is_removed_when_encoding(string keyword, string unsafeCharacters, string expected)
    {
        // Given
        var encoder = Given_an_encoder(unsafeCharacters);

        // When
        var result = encoder.Encode(keyword);

        //Then
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DefaultEncoder_has_no_unsafeCharacters()
    {
        // Given
        var encoder = Given_a_default_encoder();

        // When
        var expected = "";
        var result = encoder.UnsafeChars;

        // Then
        Assert.Equal(expected, result);
    }

    public static IEncoder Given_a_default_encoder() => new DefaultEncoder();

    public static IEncoder Given_an_encoder(string? unsafeCharacters = null)
    {
        if(unsafeCharacters == null)
        {
            return new DefaultEncoder("AOU?EIY??");
        }
        
        return new DefaultEncoder(unsafeCharacters);
    }
}