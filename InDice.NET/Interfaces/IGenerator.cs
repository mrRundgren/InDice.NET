using InDice.NET.Models;

namespace InDice.NET;
public interface IGenerator
{
    IEncoder Encoder { get; set; }
    IEnumerable<string> Generate(params string[] keywords);
    IEnumerable<Keyword> GenerateFor<T>(T entity) where T : class;
}
