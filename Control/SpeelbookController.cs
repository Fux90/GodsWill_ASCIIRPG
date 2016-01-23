using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    public interface SpeelbookController : Controller<Spellbook>
    {
        int SelectedIndex { get; }
        bool ValidIndex { get; }
        bool Opened { get; }
    }
}
