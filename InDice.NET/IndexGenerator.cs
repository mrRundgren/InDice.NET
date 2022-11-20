﻿using System.Diagnostics.SymbolStore;
using System.Reflection;
namespace InDice.NET;
public class IndexGenerator : IGenerator
{
    public IEncoder Encoder { get; set; } = new IndexEncoder();

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
        IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeywordAttribute), false));
        Dictionary<string, string> result = new();

        foreach (var prop in properties)
        {
            if(prop != null)
            {
                var val = prop.GetValue(entity);

                if (val is IIndexableEntity)
                {
                    result = result.Concat(Generate(val).Where(x => !result.Keys.Contains(x.Key))).ToDictionary(_ => _.Key, _ => _.Value);
                }
                else
                {
                    if(val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                    {
                        result = result.Concat(Generate(val?.ToString() ?? "").Where(x => !result.Keys.Contains(x.Key))).ToDictionary(_ => _.Key, _ => _.Value);
                    }
                }
            }
        }

        return result.OrderBy(_ => _.Key.Length).ToDictionary(_ => _.Key, _ => _.Value);
    }
}
