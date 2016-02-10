using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;

namespace GodsWill_ASCIIRPG
{
    /// <summary>
    /// Map Viewer
    /// </summary>
    public interface IMapViewer : IViewer
    {
        /// <summary>
        /// Notifies a movement of an Atom
        /// </summary>
        /// <param name="movedAtom">Moved Atom</param>
        /// <param name="freedCell">Cell freed by moved Atom</param>
        /// <param name="occupiedCell">Cell occupied by Atom</param>
        void NotifyMovement(Atom movedAtom, Coord freedCell, Coord occupiedCell);

        /// <summary>
        /// Notifies removal of an Atom from a cell
        /// </summary>
        /// <param name="freedCell">Cell freed by removed Atom</param>
        void NotifyRemoval(Coord freedCell);

        /// <summary>
        /// Notify an exploration of nearby locations
        /// </summary>
        void NotifyExploration();
    }
}