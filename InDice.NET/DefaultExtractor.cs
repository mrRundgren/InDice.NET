using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.NET
{
    public class DefaultExtractor : IExtractor
    {
        public IEncoder Encoder { get; set; } = null!;

        public DefaultExtractor()
        {
            Encoder = new DefaultEncoder();
        }

        public DefaultExtractor(string unsafeChars)
        {
            Encoder = new DefaultEncoder(unsafeChars);
        }

        public IEnumerable<string> ExtractExplicitMatches(string source, out string newSource) =>
            source.ExtractSubStrings(('"', '"'), Encoder, out newSource).Union(newSource.ExtractSubStrings(('+', ' '), Encoder, out newSource));

        public IEnumerable<string> ExtractImplicitMatches(string source, out string newSource)
        {
            string backup = source;

            ExtractExplicitMatches(source, out source);
            ExtractExcludedMatches(source, out source);

            newSource = backup.Difference(source);
            
            return source.Split(' ').Select(_ => Encoder.Encode(_)).Where(_ => !string.IsNullOrWhiteSpace(_));
        }

        public IEnumerable<string> ExtractExcludedMatches(string source, out string newSource) =>
            source.ExtractSubStrings(('-', ' '), Encoder, out newSource);
    }
}
