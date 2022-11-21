using InDice.NET;
using InDice.Tests.Models;
using Xunit;

namespace InDice.Tests;

public class IndexGeneratorTests
{
    [Theory]
    [InlineData("Malmö", new string[] { "M", "ML", "MLM" })]
    [InlineData("Anders", new string[] { "N", "ND", "NDR", "NDRS" })]
    [InlineData("Rungren", new string[] { "R", "RN", "RNG", "RNGR", "RNGRN" })]
    [InlineData("Anders Rundgren", new string[] { "N", "ND", "NDR", "NDRS", "NDRSR", "NDRSRN", "NDRSRND", "NDRSRNDG", "NDRSRNDGR", "NDRSRNDGRN" })]
    public void Indexes_for_single_keyword_are_generated_correctly(string keyword, string[] expected)
    {
        // Given
        var generator = Given_a_generator();

        // When
        var result = generator.Generate(keyword);

        //Then
        Assert.Equal(expected, result.Select(_ => _.Key));
    }

    [Fact]
    public void Indexes_for_entity_with_indexable_attribute_properties_are_generated_correctly()
    {
        // Given
        var (person, generator, expected) = Given_a_person_and_a_generator_and_an_expected_result();

        // When
        var result = generator.Generate(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Key));
    }

    [Fact]
    public void Indexes_for_entity_with_indexable_attribute_and_deep_childs_properties_are_generated_correctly()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();

        // When
        var result = generator.Generate(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Key));
    }

    [Fact]
    public void Generator_has_default_encoder()
    {
        // Given
        var generator = Given_a_generator();

        // Then
        Assert.NotNull(generator.Encoder);
    }

    [Fact]
    public void Encoder_can_be_assigned_on_generator()
    {
        // Given
        var generator = Given_a_generator();

        // When
        generator.Encoder = new IndexEncoder("ABC");

        // Then
        Assert.NotNull(generator.Encoder);
        Assert.Equal("ABC", generator.Encoder.UnsafeChars);
    }

    [Fact]
    public void If_keyword_is_null_no_index_is_generated()
    {
        // When
        var generator = Given_a_generator();

        //When
        var result = generator.Generate(new NullModel());

        //Then
        Assert.True(result.Count == 0);
    }

    public IGenerator Given_a_generator() =>
        new IndexGenerator("AOUÅEIYÄÖ");

    public (PersonModel person, IGenerator generator, string[] expected) Given_a_person_and_a_generator_and_an_expected_result() =>
        (new PersonModel { Firstname = "Anders", Lastname = "Rundgren", HiringNo = 100101, Office = new OfficeModel { Name = "Malmö" } }, new IndexGenerator("AOUÅEIYÄÖ"), new string[] { "N", "R", "1", "M", "ND", "RN", "10", "ML", "NDR", "RND", "100", "MLM", "NDRS", "RNDG", "1001", "RNDGR", "10010", "NDRSR", "RNDGRN", "100101", "NDRSRN", "NDRSRND", "NDRSRNDG", "NDRSRNDGR", "NDRSRNDGRN" });

    public (PersonModel person, IGenerator generator, string[] expected) Given_a_person_with_manager_and_a_generator_and_an_expected_result() =>
        (new PersonModel { Firstname = "Anders", Lastname = "Rundgren", HiringNo = 100101, Office = new OfficeModel { Name = "Malmö", Manager = new Manager { Name = "Peter Ohlman" } } }, new IndexGenerator("AOUÅEIYÄÖ"), new string[] { "N", "R", "1", "M", "P", "ND", "RN", "10", "ML", "PT", "NDR", "RND", "100", "MLM", "PTR", "NDRS", "RNDG", "1001", "PTRH", "RNDGR", "10010", "NDRSR", "PTRHL", "RNDGRN", "100101", "NDRSRN", "PTRHLM", "NDRSRND", "PTRHLMN", "NDRSRNDG", "NDRSRNDGR", "NDRSRNDGRN" });
}