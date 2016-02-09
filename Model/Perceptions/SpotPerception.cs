using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Perceptions
{
    [Serializable]
    public class SpotPerception : Perception
    {
        public SpotPerception()
        {

        }

        protected override bool SensingProcess(Character sensor, Atom sensed, int CD)
        {
            var res = Dice.Throws(20)
                        + sensor.Stats[StatsType.Mental].ModifierOfStat()
                        + sensor.TempModifiers.GetBonus<int>(TemporaryModifier.ModFor.SpotPerception,
                                                                (a, b) => a + b); ;

            var isSensed = res >= CD;

            var sensorType = sensor.GetType();
            var pgType = typeof(Pg);
            var sensedType = sensed.GetType();

            if (isSensed)
            {
                if (typeof(HiddenAtom).IsAssignableFrom(sensedType))
                {
                    var _sensed = (HiddenAtom)sensed;
                    if (_sensed.Hidden)
                    {
                        _sensed.NotifyIndividuation();
                    }
                }
                else
                {
                    if (sensorType == pgType || sensorType.IsSubclassOf(pgType))
                    {
                        var msg = "Noises in the area...";
                        sensor.NotifyListeners(msg);
                    }
                }
            }

            return isSensed;
        }
    }
}
