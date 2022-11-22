using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.NET
{
    public interface IExtractor
    {
        IEnumerable<string> ExtractExplicitMatches(string source, out string newSource);
        IEnumerable<string> ExtractImplicitMatches(string source, out string newSource);
        IEnumerable<string> ExtractExcludedMatches(string source, out string newSource);
    }
}
