using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    /// <summary>
    /// Spellbook Controller
    /// </summary>
    public interface SpellbookController : Controller<Spellbook>
    {
        /// <summary>
        /// Index of selected Spell
        /// </summary>
        int SelectedIndex { get; }

        /// <summary>
        /// If SelectedIndex is valid
        /// </summary>
        bool ValidIndex { get; }

        /// <summary>
        /// If Spellbook is opened or not
        /// </summary>
        bool Opened { get; }
    }
}
