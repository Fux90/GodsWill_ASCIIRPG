using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    public abstract class HealSpell : Spell
    {
        private List<IDamageable> targets;
        private DamageCalculator dmg;

        public HealSpell()
            : base(null, null)
        {

        }

        public HealSpell(   ISpellcaster launcher, 
                            List<IDamageable> targets,
                            DamageCalculator dmg,
                            Animation animation = null)
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
            
            targets.ForEach(t =>
            {
                var damageable = (IDamageable)t;
                damageable.HealDamage(dmg);
            });
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine(String.Format("{0} [Heal Spell]", this.Name));
            str.AppendLine(String.Format("Target: {0}", this.Target));
            str.AppendLine("Healing Effects:");
            str.AppendLine(this.dmg.ToString());

            return str.ToString();
        }
    }

    public abstract class UtilitySpell : Spell
    {
        public UtilitySpell()
            : base(null, null)
        {

        }

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

            public event HpModify Cured;
            public event HpModify Damaged;

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

        public AttackSpell()
            : base(null, null)
        {

        }

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

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine(String.Format("{0} [Attack Spell]", this.Name));
            str.AppendLine(String.Format("Target: {0}", this.Target));
            str.AppendLine("Healing Effects:");
            str.AppendLine(this.dmg.ToString());

            return str.ToString();
        }
    }

    [Serializable]
    public abstract class HealSpellBuilder<SpellToBuild> : SpellBuilder<IDamageable, SpellToBuild>
        where SpellToBuild : HealSpell
    {
        public HealSpellBuilder()
        {
            this.Targets = new List<IDamageable>() { (IDamageable)Caster };
        }

        public HealSpellBuilder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void SetTargets<T>(List<T> targets)
        {
            Targets.Clear();
            targets.ForEach(target => Targets.Add((IDamageable)target));
        }

        public override string FullDescription
        {
            get
            {
                bool issues;
                var spell = CreateForDescription(out issues);
                return spell.ToString();
            }
        }
    }

    [Serializable]
    public abstract class UtilitySpellBuilder<SpellToBuild> : SpellBuilder<Atom, SpellToBuild>
        where SpellToBuild : UtilitySpell
    {
        public UtilitySpellBuilder()
        {

        }

        public UtilitySpellBuilder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public abstract class AttackSpellBuilder<SpellToBuild> : SpellBuilder<IDamageable, SpellToBuild>
        where SpellToBuild : AttackSpell
    {
        public AttackSpellBuilder()
        {

        }

        public AttackSpellBuilder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void SetTargets<T>(List<T> targets)
        {
            Targets.Clear();
            targets.ForEach(target => Targets.Add((IDamageable)target));
        }

        public override string FullDescription
        {
            get
            {
                bool issues;
                var spell = CreateForDescription(out issues);
                return spell.ToString();
            }
        }
    }
}
