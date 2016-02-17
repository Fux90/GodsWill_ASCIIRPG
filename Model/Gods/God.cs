#define SHOW_DICE_RESULT

//#define DEBUG_ALWAY_BAD
//#define DEBUG_ALWAY_GOOD
//#define DEBUG_ALWAY_VERY_GOOD
//#define DEBUG_ALWAY_NONE

using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Gods;
using GodsWill_ASCIIRPG.Model.TemporaryBonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Gods
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

        protected PrayResult currentPrayResult;
        private int[][] _percentageOfSuccessByLevel;
        /// <summary>
        /// pg.level as int -> [thrRes_1, thrRes_2,..., thrRes_n]
        /// </summary>
        private int[][] percentageOfSuccessByLevel
        {
            get
            {
                if(_percentageOfSuccessByLevel == null)
                {
                    #region BUILDING
                    var dict = LevelPercentageToPrayResult;

                    var numLevels = Enum.GetValues(typeof(Pg.Level)).Length;
                    var numPrayResults = Enum.GetValues(typeof(PrayResult)).Length;

                    if(numLevels != dict.Keys.Count)
                    {
                        throw new Exception(String.Format("God {0} has some Pg.Level missing", Name));
                    }

                    _percentageOfSuccessByLevel = new int[numLevels][];

                    for (int curLvl = 0; curLvl < numLevels; curLvl++)
                    {
                        var dictPrayResultToPerc = dict[(Pg.Level)curLvl];
                        if (dictPrayResultToPerc.Keys.Count != numPrayResults)
                        {
                            throw new Exception(String.Format(  "God {0} has percentage missing for Pg.Level {1}", 
                                                                Name,
                                                                (Pg.Level)curLvl));
                        }
                        _percentageOfSuccessByLevel[curLvl] = new int[numPrayResults];
                        var percSuccesGivenLevel = _percentageOfSuccessByLevel[curLvl];

                        for (int curRes = 0; curRes < numPrayResults; curRes++)
                        {
                            percSuccesGivenLevel[curRes] = dictPrayResultToPerc[(PrayResult)curRes];
                        }
                    }
                    #endregion
                }

                return _percentageOfSuccessByLevel;
            }
        }

        private Dictionary<Pg.Level, MethodInfo[]> _badReactions;
        private Dictionary<Pg.Level, MethodInfo[]> _goodReactions;
        private Dictionary<Pg.Level, MethodInfo[]> _veryGoodReactions;

        private void buildDictReactions(out Dictionary<Pg.Level, MethodInfo[]>  reactions, Type type)
        {
            reactions = new Dictionary<Pg.Level, MethodInfo[]>();
           
            var _rM = from method in this.Type.GetMethods()
                      let attributes = method.GetCustomAttributes(type, false)
                      where method.GetCustomAttributes(type, false).Length > 0
                      let levels = attributes.Select(a => ((OnPrayResult)a).ForLevel).ToList()
                      select new { Method = method, ForLevel = levels };

            var numLevels = Enum.GetValues(typeof(Pg.Level)).Length;
            for (int i = 0; i < numLevels; i++)
            {
                var lvl = (Pg.Level)i;
                reactions[lvl] = _rM.Where(m => m.ForLevel.Contains(lvl)).Select(m => m.Method).ToArray();
                
                if (reactions[lvl].Length == 0)
                {
                    throw new Exception(String.Format("God {0} must have a method for {1} at level {2}",
                                            Name,
                                            type.Name,
                                            lvl));
                }
            }
        }

        private Dictionary<Pg.Level, MethodInfo[]> badReactions
        {
            get
            {
                if (_badReactions == null)
                {
                    buildDictReactions(out _badReactions, typeof(OnBadPrayResult));
                }

                return _badReactions;
            }
        }

        private Dictionary<Pg.Level, MethodInfo[]> goodReactions
        {
            get
            {
                if (_goodReactions == null)
                {
                    buildDictReactions(out _goodReactions, typeof(OnGoodPrayResult));
                }

                return _goodReactions;
            }
        }

        private Dictionary<Pg.Level, MethodInfo[]> veryGoodReactions
        {
            get
            {
                if (_veryGoodReactions == null)
                {
                    buildDictReactions(out _veryGoodReactions, typeof(OnVeryGoodPrayResult));
                }

                return _veryGoodReactions;
            }
        }

        public God()
        {
        }

        public PrayResult HearPray(IPrayer prayer)
        {
            currentPrayResult = internalPray(prayer);
            return currentPrayResult;
        }

        private PrayResult internalPray(IPrayer prayer)
        {
            // TODO: random value of PrayResul according to level and dice throw
#if DEBUG_ALWAY_BAD
            return PrayResult.Bad;
#elif DEBUG_ALWAY_GOOD
            return PrayResult.Good;
#elif DEBUG_ALWAY_VERY_GOOD
            return PrayResult.VeryGood;
#elif DEBUG_ALWAY_NONE
            return PrayResult.None;
#else
            var thrs = percentageOfSuccessByLevel[(int)prayer.CurrentLevel].ToList();
            var diceThrow = Dice.Throws(new Dice(100));
            var prayResult = thrs.IndexOf(thrs.Last(t => t < diceThrow));

#if SHOW_DICE_RESULT
            ((Atom)prayer).NotifyListeners(String.Format("{0}: {1} in [{2};{3}]",
                (PrayResult)prayResult,
                diceThrow, 
                thrs[prayResult], 
                thrs[Math.Min(prayResult+1, thrs.Count - 1)]));
#endif
            return (PrayResult)prayResult;
#endif
        }

        protected abstract Dictionary<Pg.Level, Dictionary<PrayResult, int>> LevelPercentageToPrayResult { get; }

        //public abstract PrayResult HearPray();

        public void EffectOfPray(IPrayer prayer)
        {
            switch(currentPrayResult)
            {
                case PrayResult.Bad:
                    chooseOneFrom(badReactions[prayer.CurrentLevel]).Invoke(this, new object[] { prayer });
                    break;
                case PrayResult.Good:
                    chooseOneFrom(goodReactions[prayer.CurrentLevel]).Invoke(this, new object[] { prayer });
                    break;
                case PrayResult.VeryGood:
                    chooseOneFrom(veryGoodReactions[prayer.CurrentLevel]).Invoke(this, new object[] { prayer });
                    break;
                case PrayResult.None:
                    break;
            }
        }

        private MethodInfo chooseOneFrom(MethodInfo[] methodInfos)
        {
            return methodInfos[Dice.Throws(new Dice(methodInfos.Length)) - 1];
        }
    }

    public class Gods
    {
        public  static readonly God None = null;

        public static God Ares { get { return new Ares(); } }
    }
}
