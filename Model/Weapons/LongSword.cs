using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG.Model.Weapons
{
    [Serializable]
    class LongSword : Weapon
    {
        public LongSword(string name = "Long Sword", Coord position = new Coord())
            : base(name, 
                   DefaultSymbol, 
                   Color.DarkGray, 
                   new DamageCalculator(
                       new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                       {
                           { DamageType.Physical, () => Dice.Throws(8) }
                       }),
                   description: "A long sword",
                   cost: 10,
                   weight: 2,
                   position: position)
        {

        }

        public override int BonusOnTPC { get{ return 1; } }
    }
}
