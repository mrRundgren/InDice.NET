using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.NET
{
    public interface IExtractor
    {
        IEnumerable<string> ExtractExplicits(string source);
        IEnumerable<string> ExtractImplicits(string source);
        IEnumerable<string> ExtractExclusions(string source);
    }
}
