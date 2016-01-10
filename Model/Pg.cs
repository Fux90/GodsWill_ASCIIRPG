using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GodsWill_ASCIIRPG.View;

namespace GodsWill_ASCIIRPG
{
	public class Pg : Character
	{
        public enum Level
        {
            Cleric,
            Master,
            GrandMaster
        }

        public Level CurrentLevel { get; private set; }
        private int maxLevel;

        private int[] xp;
        public int XP { get { return xp[0]; } private set { xp[0] = value; } }
        public int NextXP { get { return xp[1]; } private set { xp[1] = value; } }

        public Pg()
            : base( "Pg", 10, 10 , 10, new Stats(StatsBuilder.RandomStats()), null, null, null, new Backpack(), 
                    "@", Color.White, "Player")
        {
            CurrentLevel = Level.Cleric;
            maxLevel = Enum.GetValues(typeof(Level)).Length - 1;
            xp = new int[2];
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

        public override void Interaction(Atom interactor)
        {
            if (interactor.GetType() == typeof(AICharacter))
            {
                var aiChar = (AICharacter)interactor;
                if(aiChar.Hostile)
                {
                    this.Attack(aiChar);
                }
                else
                {
                    // Talk?
                }
            }
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