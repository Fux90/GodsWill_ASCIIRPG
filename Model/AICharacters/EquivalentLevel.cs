using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.AICharacters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EquivalentLevel : Attribute
    {
        public Pg.Level Level { get; private set; }

        public EquivalentLevel(Pg.Level level)
        {
            Level = level;        
        }
    }
}
