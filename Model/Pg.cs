using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GodsWill_ASCIIRPG.View;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Perceptions;
using GodsWill_ASCIIRPG.Model.Items;
using System.Runtime.Serialization;
using GodsWill_ASCIIRPG.Main;
using System.IO;
using GodsWill_ASCIIRPG.Model.Gods;

namespace GodsWill_ASCIIRPG
{
    public class PgCreator
    {
        #region PROPERTIES
        public string Name { get; set; }
        public Pg.Level Level { get; set; }
        public int CurrentXp { get; set; }
        public int NextXp { get; set; }
        public int CurrentPf { get; set; }
        public int MaxPf { get; set; }
        public int Hunger { get; set; }
        public Stats Stats { get; set; }
        public Armor Armor { get; set; }
        public Shield Shield { get; set; }
        public Weapon Weapon { get; set; }
        public Backpack Backpack { get; set; }
        public Spellbook Spellbook { get; set; }
        public string Symbol { get; private set; }
        public Color Color { get; private set; }
        public God God { get; set; }
        public bool Unblockable { get; set; }
        public int PerceptionRange { get; set; }
        #endregion

        public PgCreator()
        {
            Name = RandomName();
            Stats = new Stats(StatsBuilder.RandomStats());
            var toughMod = Stats[StatsType.Toughness].ModifierOfStat();
            Level = Pg.Level.Novice;
            CurrentXp = Pg.XpForLevel(Level);
            NextXp = Pg.XpForLevel(Level.Next());
            CurrentPf = Pg.healthDice.Max + toughMod;
            MaxPf = Pg.healthDice.Max + toughMod;
            Hunger = Pg.hungerDice.Max * Math.Max(1, toughMod);
            Armor = null;
            Shield = null;
            Weapon = null;
            Backpack = new Backpack();
            Spellbook = new Spellbook();
            Symbol = "@";
            Color = Color.White;
            God = null;
            Unblockable = false;
            PerceptionRange = 10;
        }
        

        private string RandomName()
        {
            return "Rogdar";
        }

        public Pg Create()
        {
            return new Pg(  Name,
                            Level,
                            CurrentXp,
                            NextXp,
                            CurrentPf,
                            MaxPf,
                            Hunger,
                            PerceptionRange,
                            Unblockable,
                            Stats,
                            Armor,
                            Shield,
                            Weapon,
                            Backpack,
                            Spellbook,
                            God,
                            Symbol,
                            Color);
        }

        public Pg CreateFromFile()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    [HasPerception(typeof(ListenPerception))]
    [HasPerception(typeof(SpotPerception))]
    public class Pg : Character, ISpellcaster, IViewable<IPgViewer>
	{
        #region SERIALIZATION_CONST_NAMES
        const string currentLevelSerializationName = "level";
        const string maxLevelSerializationName = "maxLevel";
        const string xpSerializationName = "xp";
        const string perceptionRangeSerializationName = "perceptionRange";
        const string spellbookSerializationName = "spellbook";
        #endregion

        public enum Level
        {
            Novice,
            Cleric,
            Master,
            GrandMaster
        }

        public static readonly Dice healthDice = new Dice(nFaces: 10);
        public static readonly Dice hungerDice = new Dice(nFaces: 4);

        private Level currentLevel;
        public override Level CurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }

        private int maxLevel;
        
        private int[] xp;
        public int XP
        {
            get { return xp[0]; }
            private set
            {
                if(currentLevel == currentLevel.Next())
                {
                    xp[0] = xp[1];
                }
                else
                {
                    xp[0] = value;
                }
            }
        }
        public int NextXP { get { return xp[1]; } private set { xp[1] = value; } }

        public override Dice HealthDice
        {
            get { return healthDice; }
        }

        public override Dice HungerDice
        {
            get { return hungerDice; }
        }

        public int PerceptionRange { get; protected set; }
        public int LightRange { get { return 0; } }

        private Spellbook spellbook;
        public Spellbook Spellbook { get { return spellbook; } }

        public IPgViewer view;

        public Pg(  string name,
                    Level level,
                    int currentXp,
                    int nextXp,
                    int currentPf,
                    int maxPf,
                    int hunger,
                    int perception,
                    bool unblockable,
                    Stats stats,
                    Armor armor,
                    Shield shield,
                    Weapon weapon,
                    Backpack backpack,
                    Spellbook spellbook,
                    God god,
                    string symbol,
                    Color color)
            : base( name,
                    currentPf,
                    maxPf,
                    hunger,
                    stats,
                    armor,
                    shield,
                    weapon,
                    backpack,
                    god,
                    unblockable,
                    symbol,
                    color)
        {
            currentLevel = level;
            maxLevel = Enum.GetValues(typeof(Level)).Length - 1;
            xp = new int[] { currentXp, nextXp };
            this.PerceptionRange = perception;
            this.spellbook = spellbook;
        }

        public Pg(SerializationInfo info,
                    StreamingContext context)
            : base(info, context)
        {
            currentLevel = (Pg.Level)info.GetValue(currentLevelSerializationName, typeof(Pg.Level));
            maxLevel = (int)info.GetValue(maxLevelSerializationName, typeof(int));
            xp = (int[])info.GetValue(xpSerializationName, typeof(int[]));
            PerceptionRange = (int)info.GetValue(perceptionRangeSerializationName, typeof(int));
            spellbook = (Spellbook)info.GetValue(spellbookSerializationName, typeof(Spellbook));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(currentLevelSerializationName, currentLevel, typeof(Pg.Level));
            info.AddValue(maxLevelSerializationName, maxLevel, typeof(int));
            info.AddValue(xpSerializationName, xp, typeof(int[]));
            info.AddValue(perceptionRangeSerializationName, PerceptionRange, typeof(int));
            info.AddValue(spellbookSerializationName, spellbook, typeof(Spellbook));
        }

        public override void GainExperience(int xp)
        {
            XP = XP + xp;
            CheckLevelUp();
            NotifyListeners(String.Format("Gained {0} xp", xp));
            CharacterSheets.ForEach((sheet) => sheet.NotifyXp(XP, NextXP));
        }

        private void CheckLevelUp()
        {
            if(XP >= NextXP)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            currentLevel = currentLevel.Next();
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(currentLevel, God));
            NextXP = Pg.XpForLevel(currentLevel.Next());
        }

        public override void InsertInMap(Map map, Coord newPos, bool overwrite = false)
        {
            base.InsertInMap(map, newPos, overwrite);
            Explore();
        }

        public override bool Move(Direction dir, out bool acted)
        {
            var moved = base.Move(dir, out acted);
            Explore();
            return moved;
        }

        public void Explore()
        {
            var toBeChecked = new List<Atom>();
            var pts = new FilledCircle(Position, PerceptionRange);

            foreach (Coord pt in pts)
            {
                var explored = true;

                
                if (pt == Position)
                {
                    Map.Explore(pt, explored);
                    continue;
                }
                if (pt.X >= 0 && pt.Y >= 0
                    && pt.X < Map.Width && pt.Y < Map.Height)
                {
                    if (!Map.IsFullyExplored(pt))
                    {
                        // Perception of enemies
                        if (Map[pt].HasToBeInStraightSight)
                        {
                            explored = !SomethingBlockView(pt);
                            if (!explored)
                            {
                                foreach (var perception in Perceptions)
                                {
                                    explored = perception.Sense(this, Map[pt]);
                                    if (explored)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        Map.Explore(pt, explored);
                    }

                    // Perception of hidden scenery items (e.g. traps)
                    if (typeof(HiddenAtom).IsAssignableFrom(Map[pt].GetType()))
                    {
                        foreach (var perception in Perceptions)
                        {
                            var detected = perception.Sense(this, Map[pt]);
                            if (detected)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            

            this.Map.NotifyViewersOfExploration();
        }

        public override void EffectOfTurn()
        {
            base.EffectOfTurn();
            // Hunger, etc
            CharacterSheets.ForEach((sheet) => sheet.NotifyHunger(Hunger));
        }

        public override void Die(IFighter killer)
        {
            Unblock();
            CurrentMode = Modes.AfterDeath;
            view.NotifyDeath();

            // Save story ?
            // delete all world
            // Message of death
            // Back to main menu
        }

        public override void PickUpGold(Gold gold)
        {
            base.PickUpGold(gold);
            this.NotifyAll();
        }

        public override bool Interaction(Atom interactor)
        {
            if (interactor.GetType().IsSubclassOf(typeof(AICharacter)))
            {
                var aiChar = (AICharacter)interactor;
                if(aiChar.Hostile)
                {
                    aiChar.Attack(this);

                    return true;
                }
                else
                {
                    // Talk?
                    return true;
                }
            }

            return false;
        }

        public void ScrollUpMessages()
        {
            var iScrollableType = typeof(IScrollable);
            Listeners.Where(listener => iScrollableType.IsAssignableFrom(listener.GetType()))
                    .ToList().ForEach(scrollable => ((IScrollable)scrollable).ScrollUp());
        }

        public void ScrollDownMessages()
        {
            var iScrollableType = typeof(IScrollable);
            Listeners.Where(listener => iScrollableType.IsAssignableFrom(listener.GetType()))
                    .ToList().ForEach(scrollable => ((IScrollable)scrollable).ScrollDown());
        }

        public override void RegisterTemporaryMod(TemporaryModifier mod)
        {
            base.RegisterTemporaryMod(mod);
            NotifyAll();
        }
        public override void RegisterView(ISheetViewer sheet)
        {
            base.RegisterView(sheet);
            NotifyAll();
        }

        protected override void NotifyAll()
        {
            base.NotifyAll();
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(this.currentLevel, this.God));
            CharacterSheets.ForEach((sheet) => sheet.NotifyXp(this.XP, this.NextXP));
        }

        public bool IsLightingCell(Coord coord)
        {
            if(Map.IsDark(coord))
            {
                var lRange = LightRange;
                var sqrDist = lRange * lRange;
                return coord.SquaredDistanceFrom(Position) < sqrDist;
            }

            return true;
        }

        public void CastSpell(Spell spell, out bool acted)
        {
            spell.Launch();
            acted = !spell.IsFreeAction;
        }

        public bool LearnSpell(SpellBuilder spell, int percentageOfSuccess)
        {
            if (spell.SatisfyRequisite(this))
            {
                //if(this.Spellbook.Contains(spell))
                // TO BE TESTED
                if (this.Spellbook.Where(spellKnown => spell.GetType() == spellKnown.GetType()).Count() > 0)
                {
                    NotifyListeners(String.Format("{0} already known", spell.Name));
                    return false;
                }

                var chance = Dice.Throws(new Dice(nFaces: 100));
                // Every +1 in modifiers for a d20 is 5%
                chance -= this.Stats[StatsType.Mental].ModifierOfStat() * 5;
                if (percentageOfSuccess == 100 || chance < percentageOfSuccess)
                {
                    this.Spellbook.Add(spell);
                    NotifyListeners(String.Format("{0} learnt", spell.Name));
                    return true;
                }
                else
                {
                    NotifyListeners(String.Format("Ritual has gone wrong!!", spell.Name));
                    return true;
                }
            }
            else
            {
                NotifyListeners(String.Format("Can't satisfy prerequisites to learn {0}", spell.Name));
                return false;
            }
        }

        public void ForgetSpell(SpellBuilder spell)
        {
            this.Spellbook.Remove(spell);
        }

        public void RegisterView(IPgViewer viewer)
        {
            view = viewer;
        }

        public override bool TriggerCurrent()
        {
            var triggerable = (Atom)CurrentTriggerable;
            var res = base.TriggerCurrent();

            if(res)
            {
                NotifyListeners(String.Format("Triggered {0}", triggerable.Name));
            }
            else
            {
                NotifyListeners("Nothing to activate");
            }

            return res;
        }

        public override void RegisterTriggerable(ITriggerable triggerable)
        {
            base.RegisterTriggerable(triggerable);

            if (!triggerable.ImmediateTrigger)
            {
                NotifyListeners(String.Format("Press Enter to activate {0}", ((Atom)triggerable).Name));
            }
        }

        public void PostDeathOperation()
        {
            var path = Path.Combine(Game.NeededFolders[Game.Folder.Saves],
                                             String.Format("{0}.story", Name));

            using (var w = new StreamWriter(path))
            {
                var sheet = CharacterSheets.FirstOrDefault();
                if(sheet == null)
                {
                    throw new Exception("Unexpected missing Sheet");
                }

                w.Write(this.ToString());
                Game.Current.SaveLog(w);
            }
        }

        private const string sep = "=================================================";
        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendFormat("{0} [{1}{2}]", 
                                Name, 
                                currentLevel,
                                God != Gods.None 
                                ? String.Format(" of {0}", God.Name) 
                                : "");
            str.AppendLine();
            str.AppendLine(sep);
            str.AppendLine(String.Format("Max HP: {0}", this.Hp));
            str.AppendLine(String.Format("XP: {0}", this.XP));
            str.AppendLine(String.Format("Hunger: {0}", this.Hunger));
            str.AppendLine(sep);
            str.AppendLine(String.Format("CA: {0}", this.CA));
            str.AppendLine(String.Format("Special: {0}", this.CASpecial));
            str.AppendLine(sep);
            str.Append(Stats.ToVerticalString());
            str.AppendLine(sep);
            str.AppendLine(String.Format("$: {0}", this.MyGold));
            str.AppendLine(sep);
            str.Append(String.Format("Weapon: {0}", this.HandledWepon));
            str.Append(String.Format("Shield: {0}", this.EmbracedShield));
            str.Append(String.Format("Armor: {0}", this.WornArmor));
            str.AppendLine(sep);
            str.AppendLine("BACKPACK");
            str.AppendLine(Backpack.ToString());

            return str.ToString();
        }

        private static readonly Dictionary<Level, int> xpForLevel = new Dictionary<Level, int>()
        {
            {Level.Novice, 0 },
            {Level.Cleric, 100 },
            {Level.Master, 10000 },
            {Level.GrandMaster, 100000 },
        };

        public static int XpForLevel(Level level)
        {
            return xpForLevel[level];
        }
    }
}