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
        protected FireOrb(Atom sender, Atom target)
            : base(sender, new FireBallAnimation(sender.Position, target.Position, Color.Red))
        {

        }

        public override string FullDescription
        {
            get
            {
                return "Fire orb";
            }
        }

        public FireOrb Create(Atom sender, Atom target)
        {
            return new FireOrb(sender, target);
        }

        public override void Launch()
        {
            //Launcher
        }

        protected override void Effect(AtomCollection targets, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}
