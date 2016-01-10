using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GodsWill_ASCIIRPG.Model
{
    class Floor : SceneryItem
    {
        public Floor(Coord position)
            : base("Floor", 
                   ".", 
                   Color.White, 
                   true, 
                   "A walkable tile",
                   position)
        {

        }

        public Floor()
            : this(new Coord())
        {

        }
    }
}
