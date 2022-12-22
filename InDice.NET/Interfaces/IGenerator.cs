namespace InDice.NET;
public interface IGenerator
{
    IEncoder Encoder { get; set; }
    IEnumerable<string> Generate(params string[] keywords);
    IEnumerable<(string Index, int LevenshteinDistance, double Similarity)> GenerateFor<T>(T entity) where T : class;
}
