using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    public enum RapidOperation
    {
        None,
        Handle,
        PutOn,
        Embrace,
        Use
    }

    public interface BackpackController : Controller<Backpack>
    {
        /// <summary>
        /// Rapid operation on opened backpack
        /// </summary>
        RapidOperation RapidOperation { get; }

        /// <summary>
        /// Index of selected item in backpack
        /// </summary>
        int SelectedIndex { get; }

        /// <summary>
        /// Check if current index is valid
        /// </summary>
        bool ValidIndex { get; }

        /// <summary>
        /// Status of backpack
        /// </summary>
        bool Opened { get; }
    }
}
