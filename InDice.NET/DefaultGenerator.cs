namespace InDice.NET;

public class DefaultGenerator : IGenerator
{
    public IEncoder Encoder { get; set; } = null!;

    public DefaultGenerator() : this(string.Empty) { }

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
        
        return result.Distinct();
    }

    public IEnumerable<(string Index, int LevenshteinDistance, double Similarity)> GenerateFor<T>(T entity) where T : class
    {
        IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(InDiceIncludeAttribute), false));
        IEnumerable<(string Index, int LevenshteinDistance, double Similarity)> result = new List<(string Index, int LevenshteinDistance, double Similarity)>();

        foreach (var prop in properties)
        {
            var attribute = prop.GetCustomAttribute<InDiceIncludeAttribute>()!;
            var mode = attribute.Mode;
            var includeReverse = attribute.IncludeReverseOrder;

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
                        string originalText = val!.ToString()!;
                        List<string> words = new();

                        if (mode == InDiceGenerateMode.SplitOnWords)
                        {
                            var subs = originalText.Split(' ');
                            for(int i = 0; i < subs.Length; i++)
                            {
                                words.Add(string.Join("", subs.Skip(i)));
                            }

                            if(includeReverse)
                            {
                                var reverse = subs.Reverse();
                                for (int i = 0; i < subs.Length; i++)
                                {
                                    words.Add(string.Join("", reverse.Skip(i)));
                                }
                            }
                        }
                        else
                        {
                            words.Add(originalText);
                        }

                        result = result.Union(Generate(words.ToArray())
                            .Select(index => (index, originalText.ToLevenshteinDistance(index, Encoder), originalText.ToSimilarity(index, Encoder))));
                    }
                }
            }
        }

        result = result
            .GroupBy(x => x.Index)
            .Select(_ => _.MaxBy(x => x.Similarity)!);

        return result.OrderBy(_ => _.Index.Length);
    }
}
