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
        void LearnSpell(SpellBuilder spell);
        void ForgetSpell(SpellBuilder spell);
    }
}
