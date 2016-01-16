using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GodsWill_ASCIIRPG.Model
{
    [Unphysical]
    public class SelectorCursor : MoveableAtom
    {
        Pg controller;

        public SelectorCursor()
            : base("selector", "█", Color.FromArgb(100, Color.Red), true, false, true)
        {
                
        }

        public void Show(Map map, Pg pg)
        {
            this.InsertInMap(map, pg.Position);
            controller = pg;
        }

        public void Hide()
        {
            this.Map.Remove(this);
        }

        public override bool Interaction(Atom interactor)
        {
            //
            return false;
        }
    }
}
