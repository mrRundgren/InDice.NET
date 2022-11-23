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
        public static KeywordResult ToKeywordResult(this string source, IExtractor extractor)
        {
            return new KeywordResult
            {
                Explicit = extractor.ExtractExplicits(source),
                Implicit = extractor.ExtractImplicits(source),
                Excluded = extractor.ExtractExclusions(source)
            };
        }

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
}
