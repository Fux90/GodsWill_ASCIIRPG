using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodsWill_ASCIIRPG.Model.Items;
using System.Runtime.Serialization;
using System.Drawing;
using GodsWill_ASCIIRPG.View;

namespace GodsWill_ASCIIRPG.Model
{
    public class Merchant : Atom, IGoldDealer, IViewable<IMerchantViewer>
    {
        private IMerchantViewer merchantView;
        public Backpack Backpack { get; private set; }

        public Merchant(string name,
                        string symbol,
                        Color color,
                        Backpack backpack,
                        string description = "A Merchant",
                        Coord position = new Coord())
            : base(name, symbol, color, true, false, description, position)
        {
            Backpack = backpack;
        }

        public Merchant(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        public void RegisterView(IMerchantViewer view)
        {
            merchantView = view;
        }

        public bool GiveAwayGold(int amount, out Gold gold)
        {
            gold = new Gold(amount);
            // A merchant has always cash!!
            return true;
        }

        public override bool Interaction(Atom interactor)
        {
            throw new NotImplementedException();
        }

        public void PickUpGold(Gold gold)
        {
            // Nothing...
        }
    }
}
