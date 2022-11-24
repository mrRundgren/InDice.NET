namespace InDice.NET;
public interface IGenerator
{
    IEncoder Encoder { get; set; }
    Dictionary<string, string> Generate(params string[] keywords);
    Dictionary<string, string> Generate<T>(T entity) where T : class;
}
