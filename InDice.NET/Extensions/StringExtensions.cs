using InDice.NET.Models;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;

namespace InDice.NET;

public static class StringExtensions
{
    public static double ToSimilarity(this string source, string target, IEncoder? encoder = null)
    {
        if(encoder == null)
        {
            encoder = new DefaultEncoder();
        }

        source = encoder.Encode(source);
        target = encoder.Encode(target);

        if ((source.Length == 0) && (target.Length == 0)) return 1.0;
        if (source == target) return 1.0;

        int stepsToSame = ToLevenshteinDistance(source, target, encoder);
        return Math.Round(1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)), 2, MidpointRounding.AwayFromZero);
    }
        
    public static int ToLevenshteinDistance(this string source, string target, IEncoder? encoder = null)
    {
        if (encoder == null)
        {
            encoder = new DefaultEncoder();
        }

        source = encoder.Encode(source);
        target = encoder.Encode(target);

        if (source == target) return 0;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;

        if (sourceWordCount == 0)
            return targetWordCount;

        if (targetWordCount == 0)
            return sourceWordCount;

        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceWordCount, targetWordCount];
    }

    public static string FindMatches(this string source, string search) =>
       FindMatches(source, search, ("[", "]"));

    public static string FindMatches(this string source, string search, (string lead, string trail) delimiters)
    {
        var keywords = search.ExtractExplicits().Union(search.ExtractImplicits()).Distinct();
        var words = source.Split(' ');

        foreach(var keyword in keywords)
        {
            bool add = true;

            foreach (var x in keyword.Split(' '))
            {
                if(!words.Any(_ => _.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    add = false;
                }
            }

            if(add)
            {
                var factor = 0;

                foreach (var ix in source.IndexesOfAll(keyword, true))
                {
                    var tmp = source.Substring(ix + factor, keyword.Length);
                    source = source.Remove(ix + factor, tmp.Length);
                    source = source.Insert(ix + factor, $"{delimiters.lead}{tmp}{delimiters.trail}");

                    factor = factor + (delimiters.lead.Length + delimiters.trail.Length);
                }
            }
        }

        return source;
    }

    public static int[] IndexesOfAll(this string source, string keyword, bool ignoreCase = false)
    {
        if (string.IsNullOrWhiteSpace(source) ||
            string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("String or substring is not specified.");
        }

        var indexes = new List<int>();
        int index = 0;

        while ((index = source.IndexOf(keyword, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
        {
            indexes.Add(index++);
        }

        return indexes.ToArray();
    }

    public static string ToAbsoluteString(this string source)
    {
        if (!source.Any(_ => "+-\"".Contains(_)))
        {
            source = $"\"{source}\"";
        }

        return source;
    }

    public static IEnumerable<string> ExtractExplicits(this string source, IEncoder? encoder = null) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('+')).Select(_ => encoder != null ? encoder.Encode(_[1..]) : _[1..]);

    public static IEnumerable<string> ExtractImplicits(this string source, IEncoder? encoder = null) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('-') == false && _.StartsWith('+') == false).Select(_ => encoder != null ? encoder.Encode(_) : _);

    public static IEnumerable<string> ExtractExclusions(this string source, IEncoder? encoder = null) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('-')).Select(_ => encoder != null ? encoder.Encode(_[1..]) : _[1..]);

    public static IEnumerable<string> QualifiedSplit(this string source, char delimiter, char qualifier)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            yield break;
        }
        else
        {
            char prevChar;
            char nextChar;
            char currentChar;
            bool inString = false;

            StringBuilder token = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                currentChar = source[i];

                if (i > 0) { prevChar = source[i - 1]; } 
                else { prevChar = '\0'; }


                if (i + 1 < source.Length) { nextChar = source[i + 1]; }
                else { nextChar = '\0'; }

                if (currentChar == qualifier && (prevChar == '\0' || (prevChar == delimiter || prevChar == '+' || prevChar == '-')) && !inString)
                {
                    inString = true;
                    continue;
                }

                if (currentChar == qualifier && (nextChar == '\0' || nextChar == delimiter) && inString)
                {
                    inString = false;
                    continue;
                }

                if (currentChar == delimiter && !inString)
                {
                    yield return token.ToString();
                    token = token.Remove(0, token.Length);
                    continue;
                }

                token = token.Append(currentChar);
            }

            yield return token.ToString();
        }
    }
}
