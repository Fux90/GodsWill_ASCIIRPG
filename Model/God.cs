using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.TemporaryBonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public abstract class God : TypeQueryable
    {
        public enum PrayResult
        {
            None,
            Bad,
            Good,
            VeryGood
        }

        public string Name
        {
            get
            {
                //return this.GetType().Name.Clean();
                return this.Type.Name.Clean();
            }
        }

        public God()
        {
        }

        public abstract PrayResult HearPray(out TemporaryModifier mod);
    }

    public class Gods
    {
        public  static readonly God None = null;

        public static God Ares { get { return new Ares(); } }
    }

    [Serializable]
    public class Ares : God
    {
        public override PrayResult HearPray(out TemporaryModifier mod)
        {
            mod = new DivineModifier<int>(5, TemporaryModifier.ModFor.CA, 40);
            return PrayResult.Good;
        }
    }
}
