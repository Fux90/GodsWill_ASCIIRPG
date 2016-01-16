using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;

namespace GodsWill_ASCIIRPG
{
    public interface IMapViewer : IViewer
    {
        void NotifyMovement(Atom movedAtom, Coord freedCell, Coord occupiedCell);
        void NotifyRemoval(Coord freedCell);
    }
}