using GodsWill_ASCIIRPG.Model.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Edibles
{
    [Stackable(10)]
    [Serializable]
    public class Meat : Edible
    {
        public Meat()
            : base(1, "Meat")
        {
            
        }

        public Meat(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
