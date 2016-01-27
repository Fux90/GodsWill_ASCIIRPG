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
            CurrentXp = 0;
            NextXp = 1000;
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

    [HasPerception(typeof(ListenPerception))]
	public class Pg : Character, ISpellcaster
	{
        public enum Level
        {
            Novice,
            Cleric,
            Master,
            GrandMaster
        }

        public static readonly Dice healthDice = new Dice(nFaces: 10);
        public static readonly Dice hungerDice = new Dice(nFaces: 4);

        public Level CurrentLevel { get; private set; }
        private int maxLevel;

        private int[] xp;
        public int XP { get { return xp[0]; } private set { xp[0] = value; } }
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
            CurrentLevel = level;
            maxLevel = Enum.GetValues(typeof(Level)).Length - 1;
            xp = new int[] { currentXp, nextXp };
            this.PerceptionRange = perception;
            this.spellbook = spellbook;
        }

        public override void GainExperience(int xp)
        {
            XP += xp;
            CheckLevelUp();
            CharacterSheets.ForEach((sheet) => sheet.NotifyXp(XP, NextXP));
        }

        private void CheckLevelUp()
        {
            if(false)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel = (Level)Math.Min(maxLevel, 1 + (int)CurrentLevel);
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(CurrentLevel, God));
            NextXP = ComputeNextXP();
        }

        private int ComputeNextXP()
        {
            throw new NotImplementedException();
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
                    && pt.X < Map.Width && pt.Y < Map.Height
                    && !Map.IsFullyExplored(pt))
                {

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
        public override void RegisterSheet(ISheetViewer sheet)
        {
            base.RegisterSheet(sheet);
            NotifyAll();
        }

        //private void NotifyAll()
        //{
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyName(this.Name));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(this.CurrentLevel, this.God));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyXp(this.XP, this.NextXP));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyGold(this.MyGold));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyHp(this.Hp, this.MaxHp));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyHunger(this.Hunger));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyDefences(this.CA, this.CASpecial));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(this.WornArmor));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyShield(this.EmbracedShield));
        //    CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(this.HandledWepon));
        //    CharacterSheets.ForEach((sheet) => {
        //        foreach (var stat in Stats.AllStats)
        //        {
        //            sheet.NotifyStat(stat, this.Stats[stat]);
        //        }
        //    });
        //}

        protected override void NotifyAll()
        {
            base.NotifyAll();
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(this.CurrentLevel, this.God));
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
                if(this.Spellbook.Contains(spell))
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
    }
}