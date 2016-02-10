using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    /// <summary>
    /// Controller of Pg
    /// </summary>
    public interface PgController : Controller<Pg>
    {
        /// <summary>
        /// Contains a BackpackController
        /// </summary>
        BackpackController BackpackController { get; }

        /// <summary>
        /// Contains a SpellbookController 
        /// </summary>
        SpellbookController SpellbookController { get; }
    }
}
