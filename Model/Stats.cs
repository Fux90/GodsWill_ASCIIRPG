using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public enum StatsType
    {
        Strength,
        Agility,
        Toughness,
        Mental,
        InnatePower,
        Precision
    }

    public class StatsBuilder
    {
        public static int[] RandomStats()
        {
            var stats = Stats.AllStats;
            var res = new int[stats.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = Dice.Throws(6, 4, (partial) =>
                {
                    return partial.Sum() - partial.Min();
                });
            }

            return res;
        }
    }

	public struct Stats
	{
        int[] stats;

        public int this[StatsType stat] { get { return stats[(int)stat]; } }

        public Stats(int[] stats)
        {
            var numOfStats = AllStats.Length;
            if(stats.Length != numOfStats)
            {
                throw new Exception(String.Format("Stats must be {0}", numOfStats));
            }
            this.stats = (int[])stats.Clone();
        }

        public static StatsType[] AllStats { get { return (StatsType[])Enum.GetValues(typeof(StatsType)); } }

        public void IncreaseStat(StatsType stat, int value)
        {
            stats[(int)stat] += value;
        }

        public bool OneIsLessThanZero()
        {
            for (int i = 0; i < stats.Length; i++)
            {
                if(stats[i] <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void DecreaseStat(StatsType stat, int value)
        {
            stats[(int)stat] = Math.Max(0, stats[(int)stat] - value);
        }
    }
}