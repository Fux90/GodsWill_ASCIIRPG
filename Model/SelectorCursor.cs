using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG.Model
{
    [Unphysical]
    public class SelectorCursor : MoveableAtom
    {
        private Pg controller;
        private int squaredPerceptionRange;

        public SelectorCursor()
            : base("selector", "█", Color.FromArgb(100, Color.Red), true, false, true)
        {
                
        }

        public void Show(Map map, Pg pg, int perceptionRange)
        {
            this.InsertInMap(map, pg.Position);
            controller = pg;
            squaredPerceptionRange = perceptionRange * perceptionRange;
        }

        public void Hide()
        {
            this.Map.Remove(this);
        }

        public override bool Move(Direction dir, out bool acted)
        {
            var moved = base.Move(dir, out acted);
            if(this.Position.SquaredDistanceFrom(controller.Position) > squaredPerceptionRange)
            {
                base.Move(dir.Opposite(), out acted);
                moved = false;
            }

            acted = false;
            return moved;
        }
        public override bool Interaction(Atom interactor)
        {
            //
            return false;
        }
    }
}
