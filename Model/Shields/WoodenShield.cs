using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GodsWill_ASCIIRPG.Model.Shields
{
    class WoodenShield : Shield
    {
        public WoodenShield(string name = "Wooden Shield")
            : base( name, 
                    Shield.DefaultSymbol, 
                    Color.Brown,
                    1,
                    0,
                    description: "A wooden small shield",
                    cost: 1,
                    weight: 2)
        {

        }
    }
}
