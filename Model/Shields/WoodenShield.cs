using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG.Model.Shields
{
    [Serializable]
    class WoodenShield : Shield
    {
        public WoodenShield(string name = "Wooden Shield", Coord position = new Coord())
            : base( name, 
                    Shield.DefaultSymbol, 
                    Color.Brown,
                    1,
                    0,
                    description: "A wooden small shield",
                    cost: 1,
                    weight: 2,
                    position: position)
        {

        }

        public WoodenShield(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
