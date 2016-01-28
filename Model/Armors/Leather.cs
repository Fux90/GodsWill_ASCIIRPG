using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG.Model.Armors
{
    [Serializable]
    class Leather : Armor
    {
        public Leather(string name = "Leather", Coord position = new Coord())
            : base(name,
                  Armor.DefaultSymbol,
                  Color.SaddleBrown,
                  new Damage(
                      new Dictionary<DamageType, int>()
                      {
                          { DamageType.Physical, 1 }
                      }),
                  _ArmorType.Light_Armor,
                  description: "An armor built in boiled leather",
                  cost: 2,
                  weight: 5,
                  position: position)

        {

        }

        public Leather(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
