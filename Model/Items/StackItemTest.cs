using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [Stackable(4, SellableOnlyAsFullStack = true)]
    public class StackItemTest : Item
    {
        public StackItemTest(   Color color = new Color(),
                                string description = "Stackable Item",
                                Coord position = new Coord(),
                                int uses = _UnlimitedUses)
            :base("Stack Item", "s", color: color, cost: 1, description: description, position: position, uses: uses)
        {

        }
        public override string FullDescription
        {
            get
            {
                return "Object to test stacking";
            }
        }
    }
}
