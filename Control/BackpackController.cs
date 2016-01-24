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
        RapidOperation RapidOperation { get; }
        int SelectedIndex { get; }
        bool ValidIndex { get; }
        bool Opened { get; }
    }
}
