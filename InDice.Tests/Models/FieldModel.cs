using InDice.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDice.Tests.Models
{
    [InDiceEntity]
    public class FieldModel : IIndexableEntity
    {
        [InDiceGenerate] public string Name { get; set; } = null!;
    }
}
