using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class Prerequisite : Attribute
    {
        private Pg.Level minimumLevel = Pg.Level.Novice;
        private Stats minimumStats = new Stats();

        public Pg.Level MinimumLevel
        {
            get { return minimumLevel; }
            set { minimumLevel = value; }
        }

        public Stats MinimumStats
        {
            get { return minimumStats; }
            set { minimumStats = value; }
        }

        public Prerequisite()
        {

        }

        public bool SatisfyPrerequisites(Character character)
        {
            throw new NotImplementedException();
        }
    }
}
