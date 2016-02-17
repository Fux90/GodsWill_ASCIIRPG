using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    /*
        Attribute given to random generators by item class type, i.e.
        weapons, armors etc
    */
    [AttributeUsage(AttributeTargets.Method)]
    public class Generator : Attribute
    {
    }
}
