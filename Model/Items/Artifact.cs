using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    /*
        Item that can be generated only Count times
    */
    [AttributeUsage(AttributeTargets.Class)]
    public class Artifact : Attribute
    {
        public int Count { get; private set; }

        public Artifact(int count)
        {
            Count = count;
        }

        public Artifact()
           : this(1)
        {

        }
    }
}
