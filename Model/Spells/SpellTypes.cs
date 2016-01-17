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
        private DamageCalculator dmg;
        private List<IDamageable> targets;

        public AttackSpell( Atom launcher, 
                            List<IDamageable> targets, 
                            DamageCalculator dmg,
                            Animation animation)
            : base(launcher, animation)
        {
            this.targets = targets;
            this.dmg = dmg;
        }

        protected void Effect(int tpc = -1)
        {
            var atomTargets = new AtomCollection();
            atomTargets.AddRange(targets.Select(t => ((Atom)t)).ToList());
            Effect(atomTargets, (object)new object[] { dmg.CalculateDamage(), tpc });
        }

        protected override void Effect(AtomCollection targets, object parameters)
        {
            var aParameters = (object[])parameters;

            var dmg = (Damage)aParameters[0];
            var tpc = (int)aParameters[1];

            if (tpc == -1)
            {
                targets.ForEach(t => ((IDamageable)t).SufferDamage(dmg));
            }
            else
            {
                targets.ForEach(t =>
                {
                    var damageable = (IDamageable)t;
                    if (tpc >= damageable.CASpecial)
                    {
                        damageable.SufferDamage(dmg);
                    }
                });
            }
        }
    }
}
