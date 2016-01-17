using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class Perception
    {
        protected abstract bool SensingProcess(Character sensor, Atom sensed);

        public bool Sense(Character sensor, Atom sensed)
        {

            var CDs = sensed.GetType()
                            .GetCustomAttributes(typeof(PerceptionCD), false);
            var CD = CDs.Where(a => ((PerceptionCD)a).PerceptionType == this.GetType()).FirstOrDefault();

            if(CD == null)
            {
                return false;
            }

            return SensingProcess(sensor, sensed);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PerceptionCD : Attribute
    {
        public int CD { get; private set; }
        public Type PerceptionType { get; set; }

        public PerceptionCD(int cd, Type perceptionType)
        {
            CD = cd;
            PerceptionType = perceptionType;
        }
    }
}
