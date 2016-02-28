using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.TemporaryBonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items.Potions
{
    [Serializable]
    [Identifiable(typeof(Potion), 10)]
    [Prerequisite(Pg.Level.Novice)]
    public class PotionDivineStrength : Potion
    {
        public PotionDivineStrength(Coord position = new Coord())
            : base("Divine Strength",
                  position,
                  "A potion of strength",
                  5)
        {

        }

        public PotionDivineStrength(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override string FullDescription
        {
            get
            {
                return "DESCRIPTION TO DO";
            }
        }

        public override void ActiveUse(Character user)
        {
            base.ActiveUse(user);

            var ttl = Dice.Throws(new Dice(6,2));
            var bonus = Dice.Throws(new Dice(4));

            user.RegisterTemporaryMod(new DivineModifier<Stats>(ttl,
                                                                TemporaryModifier.ModFor.Stats,
                                                                StatsBuilder.BuildStats(new Dictionary<StatsType, int>()
                                                                {
                                                                    {StatsType.Strength, bonus}
                                                                })));
        }
    }
}
