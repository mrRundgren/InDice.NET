using System.Collections;
using System.Reflection;

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

    public Dictionary<string, string> Generate(params string[] keywords)
    {
        Dictionary<string, string> result = new();

        foreach (var keyword in keywords)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var encoded = Encoder.Encode(keyword);

                for (int i = 0; i < encoded.Length; i++)
                {
                    result.Add(encoded[..(i + 1)], keyword);
                }
            }
        }
        
        return result;
    }

    public Dictionary<string, string> Generate<T>(T entity) where T : class
    {
        IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(InDiceGenerateAttribute), false));
        Dictionary<string, string> result = new();

        foreach (var prop in properties)
        {
            if(prop != null)
            {
                var val = prop.GetValue(entity);
                var isEntity = val?.GetType().IsDefined(typeof(InDiceEntityAttribute), false) ?? false;
                
                if (isEntity)
                {
                    result = result.Concat(Generate(val!).Where(x => !result.ContainsKey(x.Key))).ToDictionary(_ => _.Key, _ => _.Value);
                }
                else
                {
                    if(val is IList list)
                    {
                        foreach (var item in list)
                        {
                            result = result.Concat(Generate(item).Where(x => !result.ContainsKey(x.Key))).ToDictionary(_ => _.Key, _ => _.Value);
                        }
                    }
                    else if(!string.IsNullOrEmpty(val?.ToString() ?? ""))
                    {
                        result = result.Concat(Generate(val!.ToString()!).Where(x => !result.ContainsKey(x.Key))).ToDictionary(_ => _.Key, _ => _.Value);
                    }
                }
            }
        }

        return result.OrderBy(_ => _.Key.Length).ToDictionary(_ => _.Key, _ => _.Value);
    }
}
