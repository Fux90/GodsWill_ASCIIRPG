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
                res[i] = Dice.Throws(6, nDice: 4, countingMethod: (partial) =>
                {
                    return partial.Sum() - partial.Min();
                });
            }

            return res;
        }

        public static int[] RandomStats(Dictionary<StatsType, int> modifiers)
        {
            var stats = RandomStats();
            if (modifiers == null)
            {
                throw new Exception("Modifier can't be null");
            }
            foreach (var stat in modifiers.Keys)
            {
                stats[(int)stat] += modifiers[stat];
            }
            return stats;
        }
    }

    [Serializable]
	public struct Stats
	{
        int[] _stats;

        public int this[StatsType stat] { get { return stats[(int)stat]; } }

        private int[] stats
        {
            get
            {
                if(_stats == null)
                {
                    _stats = new int[Stats.AllStats.Length];
                }
                return _stats;
            }
        }

        public Stats(int[] stats)
        {
            var numOfStats = AllStats.Length;
            if(stats.Length != numOfStats)
            {
                throw new Exception(String.Format("Stats must be {0}", numOfStats));
            }
            this._stats = (int[])stats.Clone();
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

        public static bool operator >=(Stats s1, Stats s2)
        {
            var result = true;
            var stats = Stats.AllStats;
            foreach (var stat in stats)
            {
                result &= s1[stat] >= s2[stat];
            }
            return result;
        }

        public static bool operator <=(Stats s1, Stats s2)
        {
            var result = true;
            var stats = Stats.AllStats;
            foreach (var stat in stats)
            {
                result &= s1[stat] <= s2[stat];
            }
            return result;
        }

        public string ToVerticalString(bool onlyNotNull = false)
        {
            var str = new StringBuilder();
            var aS = Stats.AllStats;
            foreach (StatsType stat in aS)
            {
                var value = this[stat];
                if (onlyNotNull && value == 0)
                {
                    continue;
                }
                str.AppendLine(String.Format(   "{0}: {1}", 
                                                stat.ToString(),
                                                value));
            }
            return str.ToString();
        }
    }
}