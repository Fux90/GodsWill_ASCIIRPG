using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    public abstract class HealSpell : Spell
    {
        public HealSpell(Atom launcher, Animation animation = null)
            : base(launcher, animation)
        {

        }
    }

    public abstract class UtilitySpell : Spell
    {
        public UtilitySpell(Atom launcher, Animation animation)
            : base(launcher, animation)
        {

        }
    }

    public abstract class AttackSpell : Spell
    {
        Damage dmgs;

        public AttackSpell(Atom launcher, Animation animation)
            : base(launcher, animation)
        {

        }
    }
}
