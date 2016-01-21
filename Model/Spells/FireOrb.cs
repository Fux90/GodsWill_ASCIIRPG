using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    public class FireOrb : AttackSpell
    {
        protected FireOrb(ISpellcaster sender, IDamageable target, bool missedRealTarget)
            : base( sender,
                    new List<IDamageable>() { target },
                    new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Fire, () => Dice.Throws(8) }
                       }),
                    new FireBallAnimation(((Atom)sender).Position, ((Atom)target).Position, Color.Red))
        {
            var msg = new StringBuilder();

            if (missedRealTarget)
            {
                msg.AppendFormat("{0} misses his target!", ((Atom)sender).Name);
            }

            msg.AppendFormat("{0} hits {1}{2} with {3}",
                                ((Atom)sender).Name,
                                missedRealTarget ? "instead " : "",
                                ((Atom)target).Name,
                                this.Name);

            ((Atom)sender).NotifyListeners(msg.ToString());
        }

        public override string FullDescription
        {
            get
            {
                return "Fire orb";
            }
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
