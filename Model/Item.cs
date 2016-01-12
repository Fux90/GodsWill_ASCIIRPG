using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG
{
    abstract public class Item : Atom
	{
        public const int _UnlimitedUses = -1;

        int cost;
        int weight;
        int uses;

        public int Cost { get { return cost; } }
        public int Weight { get { return weight; } }
        public bool Expired { get { return uses == 0; } }
        public int Uses { get { return uses; } }

        public string ItemTypeName
        {
            get
            {
                return this.GetType().Name.Clean();
            }
        }

        public abstract string FullDescription { get; }

        public Item(string name = "Generic Item",
                    string symbol = "i",
                    Color color = new Color(),
                    bool walkable = true,
                    bool blockVision = false,
                    string description = "Base element of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 0,
                    int uses = _UnlimitedUses)
            : base(name, symbol, color, walkable, blockVision, description, position)
        {
            this.cost = cost;
            this.weight = weight;
            this.uses = uses;

            IsPickable = true;
        }

        public bool ConsumeUse()
        {
            if (uses > 0)
            {
                uses--;
            }

            return uses == 0;
        }

        public virtual void ActiveUse(Character user)
        {
            user.NotifyListeners("Mmm... It seems of no use");
        }

        public override bool Interaction(Atom interactor)
        {
            throw new NotImplementedException();
        }
    }
}