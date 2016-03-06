using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Rareness : Attribute
    {
        public RarenessValue Value { get; private set; }
        public Rareness(RarenessValue rareness)
        {
            Value = rareness;
        }
    }

    public enum RarenessValue
    {
        Common,
        Uncommon,
        Rare,
    }
}
