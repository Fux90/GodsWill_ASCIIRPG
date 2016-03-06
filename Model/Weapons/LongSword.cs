using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Model.Core;
using System.Runtime.Serialization;
using GodsWill_ASCIIRPG.Model.Items;

namespace GodsWill_ASCIIRPG.Model.Weapons
{
    public class LongSwordBuilder : ItemGenerator<LongSword>
    {
        public override LongSword GenerateTypedRandom(Pg.Level level, Coord position, RarenessValue rareness)
        {
            return new LongSword(position: position);
        }
    }

    [Serializable]
    public class LongSword : Weapon
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
}
