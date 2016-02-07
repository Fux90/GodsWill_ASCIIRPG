using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    public interface PgController : Controller<Pg>
    {
        //MapController MapController { get; }
        BackpackController BackpackController { get; }
        SpellbookController SpellbookController { get; }
    }
}
