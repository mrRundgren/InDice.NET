# InDice.NET
InDice.NET is a library that aims to simplify the creation off keywords for entities. This is achieved by applying the InDiceEntity attribute to a class, and marking properties to include with the InDiceGenerate attribute.


# InDice.NET

InDice.NET is a c# library to aid in generating keywords for poco classes.

## Installation

Use the package manager [nuget](https://www.nuget.org/packages/InDice.NET/) to install foobar.

## Usage

Create a class with the "InDiceEntity" attribute, tag properties you want to generate keywords for with the attribute "InDiceInclude"

```c#
[InDiceEntity]
public class PersonModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [InDiceInclude]
    public string Firstname { get; set; } = string.Empty;
    [InDiceInclude]
    public string Lastname { get; set; } = string.Empty;
    [InDiceInclude]
    public string DisplayName
    {
        get => $"{Firstname} {Lastname}";
    }
}
```
Then just run the Generate method on an instance of the object.

```c#
var keywords = new DefaultGenerator().Generate(person);
```

## License

[MIT](https://choosealicense.com/licenses/mit/)
