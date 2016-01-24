using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public class PrayerBookBuilder : ItemGenerator<PrayerBook>
    {
        public override PrayerBook GenerateTypedRandom(Pg.Level level)
        {
            throw new NotImplementedException();
        }
    }

    public class PrayerBook : Item
    {
        SpellBuilder spell;
        int percOfSuccess;

        public PrayerBook(SpellBuilder spell, int percOfSuccess = 100)
            : base("Prayer Book", 
                  "=", 
                  System.Drawing.Color.Brown, 
                  description: "A heavy book of ancient prayers")
        {
            this.spell = spell;
            this.percOfSuccess = percOfSuccess;
        }

        public override string FullDescription
        {
            get
            {
                var desc = new StringBuilder();
                desc.AppendLine("A book containing a ritual.");
                desc.AppendLine(String.Format("It grant the prayer with {0}", spell.Name));
                desc.AppendLine(String.Format("It has a {0}% of success", percOfSuccess));
                return desc.ToString();
            }
        }

        public override void ActiveUse(Character user)
        {
            base.ActiveUse(user);
        }
    }
}
