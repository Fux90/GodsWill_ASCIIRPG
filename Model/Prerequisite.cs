﻿using System;
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

        public override string ToString()
        {
            var meString = new StringBuilder();

            meString.AppendLine("Prerequisites:");
            meString.AppendLine(String.Format("Level: {0}", MinimumLevel.ToString()));
            meString.AppendLine(MinimumStats.ToVerticalString());

            return meString.ToString();
        }
    }
}
