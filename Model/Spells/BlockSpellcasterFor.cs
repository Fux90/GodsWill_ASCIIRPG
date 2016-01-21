using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    public class BlockSpellcasterFor
    {
        public int Turns { get; }

        public BlockSpellcasterFor(int turns)
        {
            Turns = turns;
        }
    }
}
