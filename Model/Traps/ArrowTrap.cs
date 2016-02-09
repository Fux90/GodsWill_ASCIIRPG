using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Traps
{
    [Serializable]
    [PerceptionCD(typeof(SpotPerception), 10)]
    public class ArrowTrap : Trap
    {
        public ArrowTrap(Coord position = new Coord())
            : base( name: "Arrow Trap",
                    charge: 1,
                    bonus: 1,
                    damage: new DamageCalculator(
                            new Dictionary<DamageType, DamageCalculator.DamageCalculatorMethod>()
                            {
                                { DamageType.Physical, (mod) => Dice.Throws(8, mod: mod) }
                            }),
                    description:  "A hidden crossbow that fires a bolt",
                    position: position)
        {
                
        }

        public ArrowTrap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
