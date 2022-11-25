using InDice.NET.Models;
using System.Security.Cryptography.X509Certificates;

namespace InDice.NET;

public class DefaultGenerator : IGenerator
{
    public IEncoder Encoder { get; set; } = null!;

    public DefaultGenerator() {
        Encoder = new DefaultEncoder();
    }

    public DefaultGenerator(string unsafeChars)
    {
        Encoder = new DefaultEncoder(unsafeChars);
    }

    public IEnumerable<string> Generate(params string[] keywords)
    {
        List<string> result = new();

        foreach (var keyword in keywords)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var encoded = Encoder.Encode(keyword);

                for (int i = 0; i < encoded.Length; i++)
                {
                    result.Add(encoded[..(i + 1)]);
                }
            }
        }
        
        return result;
    }

    public IEnumerable<Keyword> GenerateFor<T>(T entity) where T : class
    {
        IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(InDiceIncludeAttribute), false));
        List<Keyword> result = new List<Keyword>();

        foreach (var prop in properties)
        {
            if(prop != null)
            {
                var val = prop.GetValue(entity);
                var isEntity = val?.GetType().IsDefined(typeof(InDiceEntityAttribute), false) ?? false;
                
                if (isEntity)
                {
                    result = result.Concat(GenerateFor(val!).Where(x => !result.Any(r => r.Index.Equals(x.Index)))).ToList();
                }
                else
                {
                    if(val is IList list)
                    {
                        foreach (var item in list)
                        {
                            result = result.Concat(GenerateFor(item).Where(x => !result.Any(r => r.Index.Equals(x.Index)))).ToList();
                        }
                    }
                    else if(!string.IsNullOrEmpty(val?.ToString() ?? ""))
                    {
                        var originalText = val!.ToString()!;
                        result = result.Concat(Generate(originalText)
                            .Select(_ => new Keyword { 
                                Index = _, 
                                OriginalText = originalText, 
                                PropertyName = prop.Name })
                            .Where(x => !result.Any(r => r.Index.Equals(x.Index)))).ToList();
                    }
                }
            }
        }

        return result.OrderBy(_ => _.Index.Length);
    }
}
