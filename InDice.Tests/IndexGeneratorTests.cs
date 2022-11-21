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

        //When
        var result = generator.Encoder;

        // Then
        Assert.NotNull(result);
    }

    [Fact]
    public void Encoder_can_be_assigned_on_generator()
    {
        // Given
        var generator = Given_a_generator();

        // When
        generator.Encoder = new DefaultEncoder("ABC");
        var result = generator.Encoder;

        // Then
        Assert.NotNull(result);
        Assert.Equal("ABC", result.UnsafeChars);
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

    [Fact]
    public void Default_generator_has_encoder() 
    {
        // When
        var generator = Given_a_default_generator();

        //When
        var result = generator.Encoder;

        //Then
        Assert.NotNull(result);
    }

    public static IGenerator Given_a_default_generator() =>
        new DefaultGenerator();

    public static IGenerator Given_a_generator() =>
        new DefaultGenerator("AOUÅEIYÄÖ");

    public static (PersonModel person, IGenerator generator, string[] expected) Given_a_person_and_a_generator_and_an_expected_result() =>
        (new PersonModel { 
            Firstname = "Anders", 
            Lastname = "Rundgren", 
            HiringNo = 100101, 
            Office = new OfficeModel { 
                Name = "Malmö" 
            } 
        }, 
        new DefaultGenerator("AOUÅEIYÄÖ"), 
        new string[] { "N", "R", "1", "M", "ND", "RN", "10", "ML", "NDR", "RND", "100", "MLM", "NDRS", "RNDG", "1001", "RNDGR", "10010", "NDRSR", "RNDGRN", "100101", "NDRSRN", "NDRSRND", "NDRSRNDG", "NDRSRNDGR", "NDRSRNDGRN" });

    public static (PersonModel person, IGenerator generator, string[] expected) Given_a_person_with_manager_and_a_generator_and_an_expected_result() =>
        (new PersonModel { 
            Firstname = "Anders", 
            Lastname = "Rundgren", 
            HiringNo = 100101, 
            Office = new OfficeModel { 
                Name = "Malmö", 
                Manager = new Manager { 
                    Name = "Peter Ohlman" 
                } 
            }, 
            Fields = new List<FieldModel> { 
                new FieldModel { Name = ".NET" }, 
                new FieldModel { Name = "Javascript" } 
            } 
        }, 
        new DefaultGenerator("AOUÅEIYÄÖ"), 
        new string[] { "N", "R", "1", "M", "P", "J", "ND", "RN", "10", "ML", "PT", "NT", "JV", "NDR", "RND", "100", "MLM", "PTR", "JVS", "NDRS", "RNDG", "1001", "PTRH", "JVSC", "RNDGR", "10010", "NDRSR", "PTRHL", "JVSCR", "RNDGRN", "100101", "NDRSRN", "PTRHLM", "JVSCRP", "NDRSRND", "PTRHLMN", "JVSCRPT", "NDRSRNDG", "NDRSRNDGR", "NDRSRNDGRN" });
}