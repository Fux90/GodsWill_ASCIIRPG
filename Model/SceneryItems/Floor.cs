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
        public const string _Symbol = ".";
        public static readonly Color _Color = Color.White;

        public Floor(Coord position)
            : base("Floor", 
                   _Symbol, 
                   _Color, 
                   true, 
                   false,
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
