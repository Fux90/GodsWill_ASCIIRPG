using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;

namespace GodsWill_ASCIIRPG
{
    /// <summary>
    /// Backpack Viewer
    /// </summary>
	public interface IBackpackViewer : IViewer
	{
        /// <summary>
        /// Notifies a change in contained items
        /// </summary>
        /// <param name="items">Contained items</param>
        void NotifyAddRemoval(Item[] items);
	}
}