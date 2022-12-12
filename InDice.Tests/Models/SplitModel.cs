using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests.Models
{
    [InDiceEntity]
    public class SplitModel
    {
        [InDiceInclude(InDiceGenerateMode.SplitOnWords)]
        public string Title { get; set; } = null!;
    }

    [InDiceEntity]
    public class SplitModelThatIncludesReverseOrder
    {
        [InDiceInclude(InDiceGenerateMode.SplitOnWords, true)]
        public string Title { get; set; } = null!;
    }
}
