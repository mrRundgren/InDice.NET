using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;

namespace InDice.Tests;

public class DefaultGeneratorTests
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
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Indexes_for_entity_with_indexable_attribute_properties_are_generated_correctly()
    {
        // Given
        var (person, generator, expected) = Given_a_person_and_a_generator_and_an_expected_result();

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
    }

    [Fact]
    public void Indexes_for_entity_with_indexable_attribute_and_deep_childs_properties_are_generated_correctly()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
    }

    [Fact]
    public void Generating_keywords_sets_index()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();
        var index = "NDRS";

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
        Assert.Equal(index, result.First(_ => _.Index.Equals(index)).Index);
    }

    [Fact]
    public void Generating_keywords_sets_original_text()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();
        var originalText = "Anders";

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
        Assert.Equal(originalText, result.First().OriginalText);
    }

    [Fact]
    public void Generating_keywords_sets_property_name()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();
        var propertyName = "Firstname";

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
        Assert.Equal(propertyName, result.First().PropertyName);
    }

    [Fact]
    public void Generating_keywords_allows_to_check_match()
    {
        // Given
        var (person, generator, expected) = Given_a_person_with_manager_and_a_generator_and_an_expected_result();
        var originalText = "[An]ders";

        // When
        var result = generator.GenerateFor(person);

        // Then
        Assert.Equal(expected, result.Select(_ => _.Index));
        Assert.Equal(originalText, result.First().Match);
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
        var result = generator.GenerateFor(new NullModel());

        //Then
        Assert.True(result.Count() == 0);
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

    [Theory]
    [InlineData("the 40 year-old virgin", new string[] { "T", "4", "Y", "V", "TH", "40", "YE", "VI", "THE", "40Y", "YEA", "VIR", "THE4", "40YE", "YEAR", "VIRG", "THE40", "40YEA", "YEARO", "VIRGI", "THE40Y", "40YEAR", "YEAROL", "VIRGIN", "THE40YE", "40YEARO", "YEAROLD", "THE40YEA", "40YEAROL", "YEAROLDV", "THE40YEAR", "40YEAROLD", "YEAROLDVI", "THE40YEARO", "40YEAROLDV", "YEAROLDVIR", "THE40YEAROL", "40YEAROLDVI", "YEAROLDVIRG", "THE40YEAROLD", "40YEAROLDVIR", "YEAROLDVIRGI", "THE40YEAROLDV", "40YEAROLDVIRG", "YEAROLDVIRGIN", "THE40YEAROLDVI", "40YEAROLDVIRGI", "THE40YEAROLDVIR", "40YEAROLDVIRGIN", "THE40YEAROLDVIRG", "THE40YEAROLDVIRGI", "THE40YEAROLDVIRGIN" })]
    public void Generating_for_property_with_mode_split_generates_correct_keywords(string title, string[] expected)
    {
        // Given
        var generator = Given_a_default_generator();
        var model = new SplitModel { Title = title };

        // When
        var result = generator.GenerateFor(model).Select(_ => _.Index);

        // Then
        Assert.Equal(expected, result);
        Assert.NotNull(result.Single(_ => _.Equals("THE40")));
        Assert.NotNull(result.Single(_ => _.Equals("VIRGIN")));
        Assert.NotNull(result.Single(_ => _.Equals("YEAROLD")));
        Assert.Null(result.SingleOrDefault(_ => _.Equals("OLDYEAR")));
    }

    [Theory]
    [InlineData("Per Hansson", new string[] { "P", "H", "PE", "HA", "PER", "HAN", "PERH", "HANS", "PERHA", "HANSS", "PERHAN", "HANSSO", "PERHANS", "HANSSON", "PERHANSS", "HANSSONP", "PERHANSSO", "HANSSONPE", "PERHANSSON", "HANSSONPER" })]
    public void Generating_for_property_with_mode_split_and_reverse_generates_correct_keywords(string title, string[] expected)
    {
        // Given
        var generator = Given_a_default_generator();
        var model = new SplitModelThatIncludesReverseOrder { Title = title };

        // When
        var result = generator.GenerateFor(model).Select(_ => _.Index);

        // Then
        Assert.Equal(expected, result);
        Assert.NotNull(result.Single(_ => _.Equals("PE")));
        Assert.NotNull(result.Single(_ => _.Equals("HA")));
        Assert.NotNull(result.Single(_ => _.Equals("PER")));
        Assert.NotNull(result.Single(_ => _.Equals("PERH")));
        Assert.NotNull(result.Single(_ => _.Equals("PERHANSSON")));
        Assert.NotNull(result.Single(_ => _.Equals("HANSSONPER")));
    }

    [Theory]
    [InlineData("Per Hansson Eslöv", new string[] { "P", "H", "E", "PE", "HA", "ES", "PER", "HAN", "ESL", "PERH", "HANS", "ESLÖ", "PERHA", "HANSS", "ESLÖV", "PERHAN", "HANSSO", "ESLÖVH", "PERHANS", "HANSSON", "ESLÖVHA", "PERHANSS", "HANSSONE", "ESLÖVHAN", "HANSSONP", "PERHANSSO", "HANSSONES", "ESLÖVHANS", "HANSSONPE", "PERHANSSON", "HANSSONESL", "ESLÖVHANSS", "HANSSONPER", "PERHANSSONE", "HANSSONESLÖ", "ESLÖVHANSSO", "PERHANSSONES", "HANSSONESLÖV", "ESLÖVHANSSON", "PERHANSSONESL", "ESLÖVHANSSONP", "PERHANSSONESLÖ", "ESLÖVHANSSONPE", "PERHANSSONESLÖV", "ESLÖVHANSSONPER" })]
    public void Generating_for_property_with_mode_split_and_reverse_with_three_words_generates_correct_keywords(string title, string[] expected)
    {
        // Given
        var generator = Given_a_default_generator();
        var model = new SplitModelThatIncludesReverseOrder { Title = title };

        // When
        var result = generator.GenerateFor(model).Select(_ => _.Index);

        // Then
        Assert.Equal(expected, result);
        Assert.NotNull(result.Single(_ => _.Equals("PE")));
        Assert.NotNull(result.Single(_ => _.Equals("HA")));
        Assert.NotNull(result.Single(_ => _.Equals("PER")));
        Assert.NotNull(result.Single(_ => _.Equals("PERH")));
        Assert.NotNull(result.Single(_ => _.Equals("PERHANSSON")));
        Assert.NotNull(result.Single(_ => _.Equals("HANSSONPER")));
        Assert.NotNull(result.Single(_ => _.Equals("HANSSONESLÖV")));
        Assert.NotNull(result.Single(_ => _.Equals("ESLÖVHANSSON")));
        Assert.Null(result.SingleOrDefault(_ => _.Equals("PERESLÖV")));
    }


    public static IGenerator Given_a_default_generator() =>
        new DefaultGenerator();

    public static (IGenerator generator, SplitModel model) Given_a_default_generator_and_a_model() =>
        (new DefaultGenerator(), new SplitModel { Title = "The 40 year-old virgin" });

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