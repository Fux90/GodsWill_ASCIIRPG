using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Main;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG.Model.SceneryItems
{
    [Serializable]
    class Wall : SceneryItem
    {
        public Wall(Coord position)
            : base("Wall", 
                   "█", 
                   Color.LightGray, 
                   false, 
                   true,
                   "A rock wall",
                   position)
        {

        }

        public Wall()
            : this(new Coord())
        {

        }

        public Wall(SerializationInfo info, 
                    StreamingContext context)
            : base(info, context)
        {

        }
    }
}
