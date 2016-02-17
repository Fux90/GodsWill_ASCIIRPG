using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.TemporaryBonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Gods
{
    [Serializable]
    public class Ares : God
    {
        protected override Dictionary<Pg.Level, Dictionary<PrayResult, int>> LevelPercentageToPrayResult
        {
            get
            {
                // TO DO: Insert valid values
                #region PROBABILITIES
                return new Dictionary<Pg.Level, Dictionary<PrayResult, int>>()
                {
                    {Pg.Level.Novice, new Dictionary<PrayResult, int>()
                    {
                        {PrayResult.Bad, 0 },
                        {PrayResult.None, 10 },
                        {PrayResult.Good, 20 },
                        {PrayResult.VeryGood, 85 },
                    } },
                    {Pg.Level.Cleric, new Dictionary<PrayResult, int>()
                    {
                        {PrayResult.Bad, 0 },
                        {PrayResult.None, 10 },
                        {PrayResult.Good, 20 },
                        {PrayResult.VeryGood, 85 },
                    } },
                    {Pg.Level.Master, new Dictionary<PrayResult, int>()
                    {
                        {PrayResult.Bad, 0 },
                        {PrayResult.None, 10 },
                        {PrayResult.Good, 20 },
                        {PrayResult.VeryGood, 85 },
                    } },
                    {Pg.Level.GrandMaster, new Dictionary<PrayResult, int>()
                    {
                        {PrayResult.Bad, 0 },
                        {PrayResult.None, 10 },
                        {PrayResult.Good, 20 },
                        {PrayResult.VeryGood, 85 },
                    } },
                };
                #endregion
            }
        }

        #region NOVICE-CLERIC
        [OnGoodPrayResult(Pg.Level.Novice)]
        [OnGoodPrayResult(Pg.Level.Cleric)]
        public void GiveGoodMod(IPrayer prayer)
        {
            prayer.RegisterTemporaryMod(new DivineModifier<int>(Dice.Throws(new Dice(6)), TemporaryModifier.ModFor.CA, 2));
        }

        [OnBadPrayResult(Pg.Level.Novice)]
        [OnBadPrayResult(Pg.Level.Cleric)]
        public void GiveBadMod(IPrayer prayer)
        {
            prayer.RegisterTemporaryMod(new DivineModifier<int>(Dice.Throws(new Dice(6)), TemporaryModifier.ModFor.CA, -2));
        }

        [OnVeryGoodPrayResult(Pg.Level.Novice)]
        [OnVeryGoodPrayResult(Pg.Level.Cleric)]
        public void GiveVeryGoodMod(IPrayer prayer)
        {
            prayer.RegisterTemporaryMod(new DivineModifier<int>(Dice.Throws(new Dice(6)), TemporaryModifier.ModFor.CA, 4));
        }
        #endregion

        #region MASTER-GRANDMASTER
        [OnGoodPrayResult(Pg.Level.Master)]
        [OnGoodPrayResult(Pg.Level.GrandMaster)]
        public void GiveWeapon(IPrayer prayer)
        {
            bool issues;
            var aPrayer = (Atom)prayer;
            var positionFree = aPrayer.FirstNearFree(out issues);

            if (!issues)
            {
                var weapon = Weapon.GenerateRandom(prayer.CurrentLevel.Previous(), positionFree);
                aPrayer.Map.Insert(weapon);
            }
            else
            {
                aPrayer.NotifyListeners("No space around you to give a gift");
            }
        }

        [OnBadPrayResult(Pg.Level.Master)]
        [OnBadPrayResult(Pg.Level.GrandMaster)]
        public void GiveVeryBadMod(IPrayer prayer)
        {
            prayer.RegisterTemporaryMod(new DivineModifier<int>(Dice.Throws(new Dice(4), 2), TemporaryModifier.ModFor.CA, -15));
        }

        [OnVeryGoodPrayResult(Pg.Level.Master)]
        [OnVeryGoodPrayResult(Pg.Level.GrandMaster)]
        public void GiveSomething(IPrayer prayer)
        {
            bool issues;
            var aPrayer = (Atom)prayer;
            var positionFree = aPrayer.FirstNearFree(out issues);

            if (!issues)
            {
                var item = Item.GenerateRandom(prayer.CurrentLevel, positionFree);
                aPrayer.Map.Insert(item);
            }
            else
            {
                aPrayer.NotifyListeners("No space around you to give a gift");
            }
        }
        #endregion

        //public override PrayResult HearPray()
        //{
        //    currentPrayResult = internalPray();
        //    return currentPrayResult;
        //}
    }
}
