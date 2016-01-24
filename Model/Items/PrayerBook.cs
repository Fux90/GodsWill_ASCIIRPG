using GodsWill_ASCIIRPG.Model.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public class PrayerBookBuilder : ItemGenerator<PrayerBook>
    {
        public override PrayerBook GenerateTypedRandom(Pg.Level level, Coord position)
        {
            var allSpells = Spell.All;

            var spellsWithPrerequisite =
            from spell in allSpells
            let attributes = spell.GetCustomAttributes(typeof(Prerequisite), false)
            where attributes != null && attributes.Length > 0
            let prerequisite = ((Prerequisite)attributes[0])
            group spell by prerequisite.MinimumLevel into newGroup
            select newGroup;

            var spellsWithNoPrerequisite =
            from spell in allSpells
            let attributes = spell.GetCustomAttributes(typeof(Prerequisite), false)
            where attributes == null || attributes.Length == 0
            select spell;


            return new PrayerBook(  (SpellBuilder)Activator.CreateInstance(typeof(FireOrbBuilder)),
                                    position: position);
        }
    }

    public class PrayerBook : Item
    {
        SpellBuilder spell;
        int percOfSuccess;

        public PrayerBook(  SpellBuilder spell, 
                            int percOfSuccess = 100, 
                            Coord position = new Coord(),
                            int cost = 0,
                            int weight = 2,
                            int uses = 1)
            : base("Prayer Book", 
                  "=", 
                  System.Drawing.Color.Brown, 
                  description: "A heavy book of ancient prayers",
                  position: position,
                  cost: cost,
                  weight: weight,
                  uses: uses)
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
            if (typeof(ISpellcaster).IsAssignableFrom(user.GetType()))
            {
                var spellcasterUser = (ISpellcaster)user;
                this.spell.Caster = spellcasterUser;
                spellcasterUser.Spellbook.Add(spell);
                if(ConsumeUse())
                {
                    user.Backpack.Remove(this);
                }
            }
            else
            {
                base.ActiveUse(user);
            }
        }
    }
}
