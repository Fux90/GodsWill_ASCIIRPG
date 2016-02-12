using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.AICharacters
{
    [AttributeUsage(AttributeTargets.Class)]
    class XPPremium : Attribute
    {
        public int Value
        {
            get; private set;
        }

        public XPPremium(int value)
        {
            Value = value;
        }
    }
}
