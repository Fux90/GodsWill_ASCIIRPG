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
        public HealSpell(ISpellcaster launcher, Animation animation = null)
            : base(launcher, animation)
        {

        }
    }

    public abstract class UtilitySpell : Spell
    {
        public UtilitySpell(ISpellcaster launcher, Animation animation)
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

        public AttackSpell( ISpellcaster launcher, 
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
            Launch(atomTargets, (object)new object[] { dmg.CalculateDamage(), tpc });
        }

        protected override void Effect(AtomCollection targets, object parameters)
        {
            var aParameters = (object[])parameters;

            var dmg = (Damage)aParameters[0];
            var tpc = (int)aParameters[1];

            if (tpc == -1)
            {
                targets.ForEach(t =>
                {
                    var damageable = (IDamageable)t;
                    damageable.SufferDamage(dmg);
                    if (damageable.Dead)
                    {
                        ((Atom)Launcher).NotifyListeners(String.Format("Kills {0}",
                                                        ((Atom)t).Name));
                        damageable.Die((IFighter)Launcher);
                    }
                });
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

                    if(damageable.Dead)
                    {
                        ((Atom)Launcher).NotifyListeners(String.Format("Kills {0}", 
                                                        ((Atom)t).Name));
                        damageable.Die((IFighter)Launcher);
                    }
                });
            }
        }
    }

    public abstract class HealSpellBuilder<SpellToBuild> : SpellBuilder<ISpellcaster, SpellToBuild>
        where SpellToBuild : HealSpell
    {
        public HealSpellBuilder()
        {
            this.Targets = new List<ISpellcaster>() { Caster };
        }
    }

    public abstract class UtilitySpellBuilder<SpellToBuild> : SpellBuilder<Atom, SpellToBuild>
        where SpellToBuild : UtilitySpell
    {
        public UtilitySpellBuilder()
        {

        }
    }

    public abstract class AttackSpellBuilder<SpellToBuild> : SpellBuilder<IDamageable, SpellToBuild>
        where SpellToBuild : AttackSpell
    {
        public AttackSpellBuilder()
        {

        }

        public override void SetTargets<T>(List<T> targets)
        {
            Targets.Clear();
            targets.ForEach(target => Targets.Add((IDamageable)target));
        }
    }

}
