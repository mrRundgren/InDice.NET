using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.NET
{
    public class KeywordResult
    {
        public IEnumerable<string> Explicit { get; set; } = new List<string>();
        public IEnumerable<string> Implicit { get; set; } = new List<string>();
        public IEnumerable<string> Excluded { get; set; } = new List<string>();
    }
}
