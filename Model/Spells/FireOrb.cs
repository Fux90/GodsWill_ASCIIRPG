﻿using GodsWill_ASCIIRPG.Model.Core;
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
            return new FireOrb(sender, target);
        }

        public override void Launch()
        {
            //Launcher
            base.Effect();
        }
    }
}
