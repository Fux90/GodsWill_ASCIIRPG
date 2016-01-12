using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GodsWill_ASCIIRPG.View;
using GodsWill_ASCIIRPG.Model.Core;

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
        public string Symbol { get; private set; }
        public Color Color { get; private set; }
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
            Hunger = Pg.hungerDice.Max * toughMod;
            Armor = null;
            Shield = null;
            Weapon = null;
            Backpack = new Backpack();
            Symbol = "@";
            Color = Color.White;
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
                            CurrentPf ,
                            MaxPf,
                            Hunger,
                            Stats,
                            Armor,
                            Shield,
                            Weapon,
                            Backpack,
                            Symbol,
                            Color);
        }

        public Pg CreateFromFile()
        {
            throw new NotImplementedException();
        }
    }

	public class Pg : Character
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

        public Pg(  string name,
                    Level level,
                    int currentXp,
                    int nextXp,
                    int currentPf,
                    int maxPf,
                    int hunger,
                    Stats stats,
                    Armor armor,
                    Shield shield,
                    Weapon weapon,
                    Backpack backpack,
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
                    symbol,
                    color)
        {
            CurrentLevel = level;
            maxLevel = Enum.GetValues(typeof(Level)).Length - 1;
            xp = new int[] { currentXp, nextXp };
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
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(CurrentLevel));
            NextXP = ComputeNextXP();
        }

        private int ComputeNextXP()
        {
            throw new NotImplementedException();
        }

        public void EffectOfTurn()
        {
            // Hunger, etc
            CharacterSheets.ForEach((sheet) => sheet.NotifyHunger(Hunger));
        }

        public override void Die(Character killer)
        {
            // Save story ?
            // delete all world
            // Message of death
            // Back to main menu
        }

        public override bool Interaction(Atom interactor)
        {
            if (interactor.GetType() == typeof(AICharacter))
            {
                var aiChar = (AICharacter)interactor;
                if(aiChar.Hostile)
                {
                    this.Attack(aiChar);

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

        public override void RegisterSheet(ISheetViewer sheet)
        {
            base.RegisterSheet(sheet);
            NotifyAll();
        }

        private void NotifyAll()
        {
            CharacterSheets.ForEach((sheet) => sheet.NotifyName(this.Name));
            CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(this.CurrentLevel));
            CharacterSheets.ForEach((sheet) => sheet.NotifyXp(this.XP, this.NextXP));
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(this.Hp, this.MaxHp));
            CharacterSheets.ForEach((sheet) => sheet.NotifyHunger(this.Hunger));
            CharacterSheets.ForEach((sheet) => sheet.NotifyDefences(this.CA, this.CASpecial));
            CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(this.WornArmor));
            CharacterSheets.ForEach((sheet) => sheet.NotifyShield(this.EmbracedShield));
            CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(this.HandledWepon));
            CharacterSheets.ForEach((sheet) => {
                foreach (var stat in Stats.AllStats)
                {
                    sheet.NotifyStat(stat, this.Stats[stat]);
                }
            });
        }
    }
}