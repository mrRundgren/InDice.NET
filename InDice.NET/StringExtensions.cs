using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InDice.NET
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ExtractSubStrings(this string source, (char Start, char End) delimeters, IEncoder encoder, out string newSource)
        {
            Stack stack = new();
            List<string> result = new();
            bool found = false;
            newSource = source;

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == delimeters.Start && !found)
                {
                    stack.Push(i);
                    found = true;
                }
                else if ((source[i] == delimeters.End || i == source.Length - 1) && stack.Count > 0)
                {
                    int? pos = (int?)stack.Peek();
                    int start = (pos ?? 0) + 1;
                    int end = i - (pos ?? 0);
                    string index = source.Substring(pos ?? 0, end + 1);
                    stack.Pop();
                    result.Add(encoder.Encode(source.Substring(start, end)));
                    newSource = newSource.ReplaceFirst(index, "");
                    found = false;
                }
            }

            newSource = newSource.Replace(delimeters.Start.ToString(), "").Trim();
            return result.Where(_ => !string.IsNullOrWhiteSpace(_));
        }

        public static string ReplaceFirst(this string source, string search, string replace)
        {
            int pos = source.IndexOf(search);
            if (pos < 0)
            {
                return source;
            }
            return string.Concat(source.AsSpan(0, pos), replace, source.AsSpan(pos + search.Length));
        }

        public static string Difference(this string source, string comparer)
        {
            if (source == null)
            {
                return comparer;
            }
            if (comparer == null)
            {
                return source;
            }

            List<string> set1 = source.Split(' ').Distinct().ToList();
            List<string> set2 = comparer.Split(' ').Distinct().ToList();

            var diff = set2.Count > set1.Count ? set2.Except(set1).ToList() : set1.Except(set2).ToList();

            return string.Join(" ", diff);
        }

        public static KeywordResult ToKeywordResult(this string source, IExtractor extractor)
        {
            return new KeywordResult
            {
                Explicit = extractor.ExtractExplicitMatches(source, out source),
                Implicit = extractor.ExtractImplicitMatches(source, out source),
                Excluded = extractor.ExtractExcludedMatches(source, out _)
            };
        }
    }
}
