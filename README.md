# InDice.NET

InDice.NET is a c# library to aid in generating keywords for poco classes.

## Installation

Use the package manager [nuget](https://www.nuget.org/packages/InDice.NET/) to install InDice.NET.

## Usage

Create a class with the "InDiceEntity" attribute, tag properties you want to generate keywords for with the attribute "InDiceInclude"

### DefaultGenerator

```c#
[InDiceEntity]
public class PersonModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    [InDiceInclude(InDiceGenerateMode.SplitOnWords, true)]
    public string DisplayName
    {
        get => $"{Firstname} {Lastname}";
    }
}
```
Then just run the Generate method on an instance of the object.

```c#
var keywords = new DefaultGenerator().GenerateFor(person);
```
A list of "triples" containing the encoded keyword, similarity and Levenshtein distance will be returned.

### DefaultEncoder

The DefaultEncoder used by DefaultGenerator will remove any non letters or numbers aswell as making the string all uppercase, if you need to remove other special characters, for instance wovels this can be achieved by simply adding a string (case insensitive) containing them to the constructor of the generator.

```c#
var keywords = new DefaultGenerator("AOUÅEIYÄÖ").GenerateFor(person);
```
Beware that this will give a less accurate similarity ranking and make the match functionality not work, this might eventually be fixed, but as for now it is what it is.

## License

[MIT](https://choosealicense.com/licenses/mit/)
