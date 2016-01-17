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
        protected FireOrb(Atom sender, IDamageable target)
            : base( sender,
                    new List<IDamageable>() { target },
                    new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Fire, () => Dice.Throws(8) }
                       }),
                    new FireBallAnimation(sender.Position, ((Atom)target).Position, Color.Red))
        {

        }

        public override string FullDescription
        {
            get
            {
                return "Fire orb";
            }
        }

        public FireOrb Create(Atom sender, IDamageable target)
        {
            // Line between sender and target
            var lineOfAction = new Line(sender.Position, ((Atom)target).Position);
            // First blockable atom
            var actualTarget = target;
            foreach (Coord pt in lineOfAction)
            {
                var testedAtom = sender.Map[pt];
                if (testedAtom.Physical && !testedAtom.Walkable)
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
                }
            }
            var msg = new StringBuilder();
            var missed = actualTarget != target;
            if(missed)
            {
                msg.AppendFormat("{0} misses his target!", Launcher.Name);
                missed = true;
            }
            
            msg.AppendFormat("{0} hits {1}{2} with {3}",
                                Launcher.Name,
                                missed ? "instead " : "",
                                ((Atom)target).Name,
                                this.Name);
            
            sender.NotifyListeners(msg.ToString());

            return new FireOrb(sender, actualTarget);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
