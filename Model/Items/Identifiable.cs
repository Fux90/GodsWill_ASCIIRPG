using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Identifiable : Attribute
    {
        public int CD { get; private set; }
        public Type MacroType { get; private set; }

        public Identifiable(Type macroType, int cd)
        {
            MacroType = macroType;
            CD = cd;
        }
    }
}
