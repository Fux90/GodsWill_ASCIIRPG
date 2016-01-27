using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	class SceneryItem : Atom
	{
        public SceneryItem( string name,
                            string symbol = "*",
                            Color color = new Color(),
                            bool walkable = true,
                            bool blockVision = false,
                            string description = "Base scenario element of the game",
                            Coord position = new Coord())
            : base(name, symbol, color, walkable, blockVision, description, position)
        {

        }

        public SceneryItem( SerializationInfo info,
                            StreamingContext context)
            : base(info, context)
        {

        }

        public override bool Interaction(Atom interactor)
        {
            if (interactor.GetType() == typeof(Pg))
            {
                interactor.NotifyListeners(String.Format("It's a {0}", this.Name));
            }

            return false;
        }
    }
}