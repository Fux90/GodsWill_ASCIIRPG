using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface ISpellcaster : IStatsProvided, IBlockable
    {
        void LaunchSpell(Spell spell, out bool acted);
    }
}
