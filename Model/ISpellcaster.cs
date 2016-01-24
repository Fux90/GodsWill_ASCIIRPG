using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface ISpellcaster : IStatsProvided, IBlockable
    {
        Spellbook Spellbook { get; }
        void CastSpell(Spell spell, out bool acted);
        bool LearnSpell(SpellBuilder spell, int percentageOfSuccess = 100);
        void ForgetSpell(SpellBuilder spell);
    }
}
