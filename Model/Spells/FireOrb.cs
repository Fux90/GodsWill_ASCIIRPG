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
    [PercentageOfSuccess(100)]
    [Serializable]
    public class FireOrbBuilder : AttackSpellBuilder<FireOrb>
    {
        public FireOrbBuilder()
        {

        }

        public FireOrbBuilder(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
        }

        public override FireOrb InnerCreate(out bool issues, bool createForDescription)
        {
            issues = false;

            if (createForDescription)
            {
                return FireOrb.Create();
            }

            if (Targets.Count > 0)
            {
                return  FireOrb.Create(Caster, Targets[0]);
            }
            else
            {
                issues = true;
                return null;
            }
        }
    }

    [Target(TargetType.NumberOfTargets, NumericParameter = 1)]
    [BlockSpellcasterFor(3)]
    public class FireOrb : AttackSpell
    {
        protected static readonly DamageCalculator dC = new DamageCalculator(
                                                        new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                                                        {
                                                            { DamageType.Fire, (mod) => Dice.Throws(8, mod: mod) }
                                                        });
        protected FireOrb()
            : base(null, null, FireOrb.dC, null)
        {

        }

        protected FireOrb(ISpellcaster caster, IDamageable target, bool missedRealTarget)
            : base( caster,
                    new List<IDamageable>() { target },
                    FireOrb.dC,
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

        public static FireOrb Create()
        {
            return new FireOrb();
        }

        public static FireOrb Create(ISpellcaster sender, IDamageable target)
        {
            // Line between sender and target
            var lineOfAction = new Line(((Atom)sender).Position, ((Atom)target).Position);
            // First blockable atom
            var actualTarget = target;
            foreach (Coord pt in lineOfAction)
            {
                var testedAtom = ((Atom)sender).Map[pt];
                if (testedAtom.Physical && !testedAtom.Walkable && testedAtom != sender)
                {
                    if(typeof(IDamageable).IsAssignableFrom(testedAtom.GetType()))
                    {
                        actualTarget = (IDamageable)testedAtom;
                    }
                    else
                    {
                        actualTarget = new FakeTarget(  testedAtom.Name, 
                                                        testedAtom.Symbol, 
                                                        testedAtom.Color,
                                                        testedAtom.Position);
                    }

                    break;
                }
            }
            
            var missed = actualTarget != target;
            return new FireOrb(sender, actualTarget, missed);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
