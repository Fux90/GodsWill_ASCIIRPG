using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public abstract class Perception : TypeQueryable
    {
        protected abstract bool SensingProcess(Character sensor, Atom sensed, int CD);

        public bool Sense(Character sensor, Atom sensed)
        {

            var CDs = sensed.GetType()
                            .GetCustomAttributes(typeof(PerceptionCD), false);
            //var sensing = CDs.Where(a => ((PerceptionCD)a).PerceptionType == this.GetType()).FirstOrDefault();
            var sensing = CDs.Where(a => ((PerceptionCD)a).PerceptionType == this.Type).FirstOrDefault();

            if (sensing == null)
            {
                return false;
            }

            return SensingProcess(  sensor, 
                                    sensed,
                                    ((PerceptionCD)sensing).CD);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PerceptionCD : Attribute
    {
        public int CD { get; private set; }
        public Type PerceptionType { get; private set; }

        public PerceptionCD(Type perceptionType, int cd)
        {
            CD = cd;
            PerceptionType = perceptionType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HasPerception : Attribute
    {
        public Type PerceptionType { get; private set; }

        public HasPerception(Type perceptionType)
        {
            PerceptionType = perceptionType;
        }

        public Perception Instantiate()
        {
            return (Perception)Activator.CreateInstance(PerceptionType);
        }
    }
}
