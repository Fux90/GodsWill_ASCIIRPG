using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [Serializable]
    public class Gold : Atom
    {
        public int Amount { get; private set; }

        public Gold(int amount, Coord position = default(Coord)) 
                    : base("Gold", "$", Color.Gold, false, false, "A pocket of gold", position)
        {
            this.Amount = amount;
        }

        public override bool Interaction(Atom interactor)
        {
            if (typeof(IMerchant).IsAssignableFrom(interactor.GetType()))
            {
                ((IMerchant)interactor).PickUpGold(this);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
