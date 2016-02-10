using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    [PercentageOfSuccess(100)]
    //[Prerequisite(MinimumStats = new Stats())]
    [Serializable]
    public class CureLightWoundsBuilder : HealSpellBuilder<CureLightWounds>
    {
        // CONSTRUCTORS
        public CureLightWoundsBuilder()
            : base()
        {

        }

        public CureLightWoundsBuilder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override CureLightWounds InnerCreate(out bool issues, bool createForDescription)
        {
            issues = false;
		    return createForDescription 
                ? CureLightWounds.Create()
                : CureLightWounds.Create(Caster);
        }
    }

    [Target(TargetType.Personal)]
    [MoneyValue(10)]
    public class CureLightWounds : HealSpell
    {
        protected static readonly DamageCalculator dC = new DamageCalculator(new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                                                        {
                                                            {DamageType.Holy, (mod) => Dice.Throws(new Dice(8), mod: mod)}
                                                        });
        protected CureLightWounds()
            : base(null, null, dC)
        {

        }

        protected CureLightWounds(ISpellcaster caster)
            : base( caster, 
                    new List<IDamageable> { (IDamageable)caster },
                    CureLightWounds.dC,
                    new GlowingAnimation((Atom)caster))
        {
            if(caster != null)
            {
                ((Atom)caster).NotifyListeners(String.Format("Uses {0}", Name));
            }
        }

        public static CureLightWounds Create()
        {
            return new CureLightWounds();
        }

        public static CureLightWounds Create(ISpellcaster sender)
        {
            return new CureLightWounds(sender);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
