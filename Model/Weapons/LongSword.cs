﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Model.Core;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG.Model.Weapons
{
    [Serializable]
    class LongSword : Weapon
    {
        public LongSword(   string name = "Long Sword", 
                            Coord position = new Coord(),
                            _SpecialAttack specialAttack = null,
                            string specialAttackDescription = null)
            : base(name, 
                   DefaultSymbol, 
                   Color.DarkGray, 
                   new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Physical, (mod) => Dice.Throws(8, mod: mod) }
                       }),
                   description: "A long sword",
                   specialAttack: specialAttack,
                   specialAttackDescription: specialAttackDescription,
                   cost: 10,
                   weight: 2,
                   position: position)
        {

        }

        public LongSword(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override int BonusOnTPC { get{ return 1; } }
    }

    [Serializable]
    class FlamingLongSword : LongSword
    {
        public FlamingLongSword(string name = "Flaming Long Sword", Coord position = new Coord())
            : base(name,
                   position: position,
                   specialAttack: Weapon.WeaponSpecialAttacks.Flaming,
                   specialAttackDescription: Weapon.WeaponSpecialAttacks.Flaming.WeaponDescription())
        {
            
        }

        public FlamingLongSword(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override int BonusOnTPC { get { return 2; } }
    }
}
