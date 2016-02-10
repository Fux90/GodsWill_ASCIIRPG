using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public class PrayerBookBuilder : ItemGenerator<PrayerBook>
    {
        public override PrayerBook GenerateTypedRandom(Pg.Level casterLevel, Coord position)
        {
            var allSpells = Spell.All;

            var spellsWithPrerequisite =
            from spell in allSpells
            let attributes = spell.GetCustomAttributes(typeof(Prerequisite), false)
            where attributes != null && attributes.Length > 0
            let prerequisite = ((Prerequisite)attributes[0])
            group spell by prerequisite.MinimumLevel into newGroup
            select newGroup;

            #region PERC_LEVEL
            // Perc -> Level
            Dictionary<int, Pg.Level> percSpellLevel = null;

            switch (casterLevel)
            {
                case Pg.Level.Novice:
                    percSpellLevel = new Dictionary<int, Pg.Level>()
                    {
                        {80, Pg.Level.Novice},
                        {100, Pg.Level.Cleric},
                    };
                    break;
                case Pg.Level.Cleric:
                    percSpellLevel = new Dictionary<int, Pg.Level>()
                    {
                        {60, Pg.Level.Novice},
                        {85, Pg.Level.Cleric},
                        {100, Pg.Level.Master},
                    };
                    break;
                case Pg.Level.Master:
                    percSpellLevel = new Dictionary<int, Pg.Level>()
                    {
                        {50, Pg.Level.Novice},
                        {80, Pg.Level.Cleric},
                        {95, Pg.Level.Master},
                        {100, Pg.Level.GrandMaster},
                    };
                    break;
                case Pg.Level.GrandMaster:
                    percSpellLevel = new Dictionary<int, Pg.Level>()
                    {
                        {35, Pg.Level.Novice},
                        {65, Pg.Level.Cleric},
                        {85, Pg.Level.Master},
                        {100, Pg.Level.GrandMaster},
                    };
                    break;
            }

            #endregion

            #region LEVEL_EMPTY
            // Level -> Perc Empty Book
            Dictionary<Pg.Level, int> spellLevelPercEmpty = new Dictionary<Pg.Level, int>()
            {
                {Pg.Level.Novice, 15},
                {Pg.Level.Cleric, 10},
                {Pg.Level.Master, 5},
                {Pg.Level.GrandMaster, 1},
            };


            #endregion

            var diceResult = Dice.Throws(new Dice(nFaces: 100));
            var spellLevel = percSpellLevel.Where(p => diceResult < p.Key).Select(p => p.Value).First();

            List<Type> spellSetFromWhichToChoose = null;
            foreach (var spellGroup in spellsWithPrerequisite)
            {
                if (spellGroup.Key == spellLevel)
                {
                    spellSetFromWhichToChoose = spellGroup.ToList<Type>();
                    break;
                }
            }
            if (spellLevel == Pg.Level.Novice)
            {
                var spellsWithNoPrerequisite =
                from spell in allSpells
                let attributes = spell.GetCustomAttributes(typeof(Prerequisite), false)
                where attributes == null || attributes.Length == 0
                select spell;

                if (spellSetFromWhichToChoose == null)
                {
                    spellSetFromWhichToChoose = spellsWithNoPrerequisite.ToList<Type>();
                }
                else
                {
                    spellSetFromWhichToChoose.AddRange(spellsWithNoPrerequisite);
                }
            }

            // If spell of Novice, there's a chance book is empty
            var voidBook = Dice.Throws(new Dice(100)) <= spellLevelPercEmpty[casterLevel];

            if(voidBook || spellSetFromWhichToChoose == null || spellSetFromWhichToChoose.Count == 0)
            {
                return new VoidPrayerBook();
            }

            var ix = Dice.Throws(spellSetFromWhichToChoose.Count) - 1;

            var spellB = (SpellBuilder)Activator.CreateInstance(spellSetFromWhichToChoose[ix]);
            var pl = spellSetFromWhichToChoose[ix].GetCustomAttributes(typeof(PercentageOfSuccess), false);
            var percentageOfSuccess = pl.Length == 0 ? 100 : ((PercentageOfSuccess)pl[0]).Percentage;
            return new PrayerBook(  spellB,
                                    cost: spellB.MoneyValue,
                                    percOfSuccess: percentageOfSuccess,
                                    position: position);
        }
    }

    [Serializable]
    public class PrayerBook : Item, ISerializable
    {
        const string spellSerializableName = "spell";
        const string percOfSuccessSerializableName = "percSuccess";

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

        public PrayerBook(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.spell = (SpellBuilder)info.GetValue(spellSerializableName, typeof(SpellBuilder));
            this.percOfSuccess = (int)info.GetValue(percOfSuccessSerializableName, typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(  spellSerializableName,
                            spell,
                            typeof(SpellBuilder));
            info.AddValue(  percOfSuccessSerializableName,
                            percOfSuccess,
                            typeof(int));
        }

        public override string FullDescription
        {
            get
            {
                var desc = new StringBuilder();
                desc.AppendLine("A book containing a ritual.");
                desc.AppendLine(String.Format("It grant the prayer with {0}", spell.Name));
                desc.AppendLine(String.Format("Learning it has a {0}% of success", percOfSuccess));
                desc.AppendLine();
                desc.AppendLine(spell.Prerequisites.ToString());
                return desc.ToString();
            }
        }

        public override void ActiveUse(Character user)
        {
            if (typeof(ISpellcaster).IsAssignableFrom(user.GetType()))
            {
                var spellcasterUser = (ISpellcaster)user;
                this.spell.Caster = spellcasterUser;
                if (spellcasterUser.LearnSpell(spell, this.percOfSuccess))
                {
                    //user.NotifyListeners(String.Format("Learnt {0}", spell.Name));
                    if (ConsumeUse())
                    {
                        user.Backpack.Remove(this);
                    }
                }
                else
                {
                    user.NotifyListeners(String.Format("Can't learn {0}", spell.Name));
                }
            }
            else
            {
                base.ActiveUse(user);
            }
        }
    }

    [Serializable]
    public class VoidPrayerBook : PrayerBook
    {
        public VoidPrayerBook(Coord position = new Coord())
            : base(null,
                  position: position,
                  cost: 1,
                  weight: 2,
                  uses: Item._UnlimitedUses)
        {
            
        }

        public VoidPrayerBook(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        public override string FullDescription
        {
            get
            {
                var desc = new StringBuilder();
                desc.AppendLine("A void book.");

                return desc.ToString();
            }
        }

        public override void ActiveUse(Character user)
        {
            user.NotifyListeners(String.Format("It's an empty book... What should I read?"));
        }
    }
}
