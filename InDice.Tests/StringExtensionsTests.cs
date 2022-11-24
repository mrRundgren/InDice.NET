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
}
