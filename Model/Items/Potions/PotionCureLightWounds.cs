using GodsWill_ASCIIRPG.Model.Core;
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
    public class PotionCureLightWounds : Potion
    {
        public PotionCureLightWounds(Coord position = new Coord())
            : base("Cure Light Wounds",
                  position, 
                  "A light potion of cure",
                  5)
        {

        }

        public PotionCureLightWounds(SerializationInfo info, StreamingContext context)
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

            var cure = Dice.Throws(8);

            user.HealDamage(
                new Damage(
                    new Dictionary<DamageType, int>()
                    {
                        {DamageType.Holy, cure}
                    }));
        }
    }
}
