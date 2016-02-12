using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class Prerequisite : Attribute
    {
        private Pg.Level minimumLevel = Pg.Level.Novice;
        private Stats minimumStats = new Stats();

        public Pg.Level MinimumLevel
        {
            get { return minimumLevel; }
            private set { minimumLevel = value; }
        }

        public Stats MinimumStats
        {
            get { return minimumStats; }
            private set { minimumStats = value; }
        }

        public Prerequisite()
        {

        }

        public Prerequisite(Pg.Level minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }

        public Prerequisite(StatsType statsType, int value)
        {
            minimumStats.IncreaseStat(statsType, value);
        }

        public bool SatisfyPrerequisites(Character character)
        {
            // TO BE TESTED
            bool satisfied = true;
            if (typeof(Pg).IsAssignableFrom(character.GetType()))
            {
                satisfied &= character.CurrentLevel >= MinimumLevel;
            }
            satisfied &= character.Stats >= MinimumStats;
            return satisfied;
        }

        public override string ToString()
        {
            var meString = new StringBuilder();

            meString.AppendLine("Prerequisites:");
            meString.AppendLine(String.Format("Level: {0}", MinimumLevel.ToString()));
            meString.AppendLine(MinimumStats.ToVerticalString(true));

            return meString.ToString();
        }
    }
}
