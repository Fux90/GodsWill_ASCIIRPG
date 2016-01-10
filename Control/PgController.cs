using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    interface PgController : Controller<Pg>
    {
        //MapController MapController { get; }
        BackpackController BackpackController { get; }
    }
}
