using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Stackable : Attribute
    {
        public int MaxInStack { get; private set; }
        public bool HasStackLimit
        {
            get
            {
                return MaxInStack != -1;
            }
        }
        public bool SellableOnlyAsFullStack{ get; set; }

        public Stackable()
            : this(-1)
        {

        }

        public Stackable(int maxInStack)
        {
            MaxInStack = maxInStack;
        }
    }
}
