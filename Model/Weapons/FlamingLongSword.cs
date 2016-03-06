using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Weapons
{
    public class FlamingLongSwordBuilder : ItemGenerator<FlamingLongSword>
    {
        public override FlamingLongSword GenerateTypedRandom(Pg.Level level, Coord position, RarenessValue rareness)
        {
            return new FlamingLongSword(position: position);
        }
    }

    [Serializable]
    public class FlamingLongSword : LongSword
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
