using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Perceptions
{
    [Serializable ]
    public class ListenPerception : Perception
    {
        public ListenPerception()
        {

        }

        protected override bool SensingProcess(Character sensor, Atom sensed, int CD)
        {
            var res = Dice.Throws(20) 
                        + sensor.Stats[StatsType.Mental].ModifierOfStat() 
                        + sensor.TempModifiers.GetBonus<int>(  TemporaryModifier.ModFor.ListenPerception,
                                                                (a, b) => a + b); ;

            var isSensed = res >= CD;

            var sensorType = sensor.GetType();
            var pgType = typeof(Pg);

            if (isSensed 
                && (    sensorType == pgType
                        || sensorType.IsSubclassOf(pgType)))
            {
                var msg = "Noises in the area...";
                sensor.NotifyListeners(msg);
            }

            return isSensed;
        }
    }
}
