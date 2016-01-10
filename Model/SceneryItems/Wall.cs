using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Main;

namespace GodsWill_ASCIIRPG.Model.SceneryItems
{
    class Wall : SceneryItem
    {
        public Wall(Coord position)
            : base("Wall", 
                   "█", 
                   Color.LightGray, 
                   false, 
                   "A rock wall",
                   position)
        {

        }

        public Wall()
            : this(new Coord())
        {

        }
    }
}
