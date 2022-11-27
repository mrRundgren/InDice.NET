using static System.Formats.Asn1.AsnWriter;

namespace InDice.NET;

public static class StringExtensions
{
    public static string ToMatchedString(this string source, string index) =>
        ToMatchedString(source, index, ('[', ']'));

    public static string ToMatchedString(this string source, string index, (char lead, char trail) delimiters)
    {
        var stack = new Stack(index.Reverse().ToArray());
        int end = 0;

        for(int i = 0; i < source.ToArray().Length; i++)
        {
            if(stack.Count > 0)
            {
                end = i + 1;
                if (char.ToUpperInvariant(source[i]).Equals(stack.Peek()))
                {
                    stack.Pop();
                    continue;
                }
            }
            else
            {
                end = i;
                break;
            }
        }

        return string.Concat(delimiters.lead, source.Insert(end, delimiters.trail.ToString()));
    }

    public static string ToSearchString(this string source)
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
