using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PercentageOfSuccess : Attribute
    {
        public int Percentage { get; private set; }
        public PercentageOfSuccess(int percentage)
        {
            Percentage = percentage;
        }
    }
}
