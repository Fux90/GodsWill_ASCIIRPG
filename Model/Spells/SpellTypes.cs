using System;
using System.Collections.Generic;
using System.Drawing;
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
        protected class FakeTarget : Atom, IDamageable
        {
            public FakeTarget(string name, string symbol, Color color, Coord position)
                : base(name, symbol, color, false, false, position: position)
            {

            }

            public int CA { get; }
            public int CASpecial { get; }
            public bool Dead{get;}
            public int Hp { get; }
            public int MaxHp { get; }
            public Stats Stats { get; }

            public void DecreaseMaxHp(int deltaHp) { }
            public void Die(IFighter killer) { }
            public void DisembraceShield() { }
            public void EmbraceShield(Item shield) { }
            public void HealDamage(Damage dmg) { }
            public void IncreaseMaxHp(int deltaHp) { }
            public void RemoveArmor() { }
            public void SufferDamage(Damage dmg) { }
            public void WearArmor(Item armor) { }

            public override bool Interaction(Atom interactor) { return false; }
        }

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
