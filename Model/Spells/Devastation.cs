using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    [PercentageOfSuccess(80)]
    [Prerequisite(MinimumLevel = Pg.Level.Cleric)]
    [Serializable]
    public class DevastationBuilder : AttackSpellBuilder<Devastation>
    {
        public DevastationBuilder()
        {

        }

        public DevastationBuilder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override Devastation InnerCreate(out bool issues)
        {
            if (Targets.Count > 0)
            {
                issues = false;
                return Devastation.Create(Caster, Targets);
            }
            else
            {
                issues = true;
                return null;
            }
        }

        public override string FullDescription
        {
            get
            {
                return "Devastation";
            }
        }
    }

    [Target(TargetType.AllEnemiesInRange, NumericParameter = 3)]
    [MoneyValue(10)]
    public class Devastation : AttackSpell
    {
        protected Devastation(ISpellcaster caster, List<IDamageable> targets)
            : base( caster,
                    targets,
                    new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Necrotic, (mod) => Dice.Throws(6, 1, mod: mod) }
                       }),
                    new BlobsAnimation( targets.Select(t => ((Atom)t).Position).ToList(), 
                                        4,
                                        Color.DarkGray))
        {
            var msg = new StringBuilder();

            msg.AppendFormat("{0} hits ", ((Atom)caster).Name);

            var sep = "";
            foreach (var target in targets)
            {
                msg.AppendFormat("{0}{1}",
                                sep,
                                ((Atom)target).Name);
                sep = " and ";
            }
            msg.AppendFormat(" with {0}", this.Name);

            ((Atom)caster).NotifyListeners(msg.ToString());
        }

        public static Devastation Create(ISpellcaster sender, List<IDamageable> targets)
        {
            // Always hits
            return new Devastation(sender, targets);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
