using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Gods
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OnPrayResult : Attribute
    {
        public Pg.Level ForLevel { get; private set; }

        public OnPrayResult(Pg.Level forLevel)
        {
            ForLevel = forLevel;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnBadPrayResult : OnPrayResult
    {
        public OnBadPrayResult(Pg.Level forLevel)
            : base(forLevel)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnGoodPrayResult : OnPrayResult
    {
        public OnGoodPrayResult(Pg.Level forLevel)
            : base(forLevel)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnVeryGoodPrayResult : OnPrayResult
    {
        public OnVeryGoodPrayResult(Pg.Level forLevel)
            : base(forLevel)
        {

        }
    }
}
