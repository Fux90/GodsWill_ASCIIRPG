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
                return Devastation.Create(Caster, Targets[0]);
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

    public class Devastation : AttackSpell
    {
        protected Devastation(ISpellcaster caster, IDamageable target, bool missedRealTarget)
            : base(caster,
                    new List<IDamageable>() { target },
                    new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Necrotic, (mod) => Dice.Throws(6, 2, mod: mod) }
                       }),
                    new FireBallAnimation(((Atom)caster).Position, ((Atom)target).Position, Color.Red))
        {
            var msg = new StringBuilder();

            if (missedRealTarget)
            {
                msg.AppendFormat("{0} misses his target!", ((Atom)caster).Name);
            }

            msg.AppendFormat("{0} hits {1}{2} with {3}",
                                ((Atom)caster).Name,
                                missedRealTarget ? "instead " : "",
                                ((Atom)target).Name,
                                this.Name);

            ((Atom)caster).NotifyListeners(msg.ToString());
        }

        public static Devastation Create(ISpellcaster sender, IDamageable target)
        {
            // Always hits
            return new Devastation(sender, target, missedRealTarget: false);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
