using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
	abstract class AICharacter : Character
	{
        public delegate int XPCalculationMethod(Character pg, Character monster);

        private bool hostile;
        private AI intelligence;
        
        public bool Hostile { get { return hostile; } }
        public AI Intelligence { get { return intelligence; } }
        public int PerceptionDistance { get; protected set; }

        public AICharacter( string name, 
                            int currentPf,
                            int maximumPf,
                            int hunger,
                            AI intelligence, 
                            int perceptionDistance, 
                            Stats? stats = null,
                            Armor wornArmor = null,
                            Shield embracedShield = null,
                            Weapon handledWeapon = null,
                            Backpack backpack = null,
                            string symbol = "C",
                            Color? color = null,
                            string description = "A creature of the world", 
                            Coord position = new Coord(),
                            bool hostile = true)
            : base( name, 
                    currentPf,
                    maximumPf,
                    hunger,
                    stats == null ? new Stats(StatsBuilder.RandomStats()) : (Stats)stats,
                    wornArmor,
                    embracedShield, 
                    handledWeapon, 
                    backpack == null ? new Backpack() : backpack,
                    symbol,
                    color == null 
                        ? (hostile ? Color.Red : Color.Green)
                        : (Color)color,
                    description,
                    position)
        {
            this.intelligence = intelligence;
            this.PerceptionDistance = perceptionDistance;
            this.hostile = hostile;
        }

        private static XPCalculationMethod xpCalculation;
        public static XPCalculationMethod XpCalculation
        {
            get
            {
                return xpCalculation != null ? xpCalculation : (pg, monster) => 0;
            }
            protected set
            {
                xpCalculation = value;
            }
        }

        public override void Interaction(Atom interactor)
        {
            if(interactor.GetType() == typeof(Pg) && hostile)
            {
                var pg = interactor as Pg;
                pg.Attack(this);
            }
        }

        public virtual bool SensePg(Pg pg)
        {
            return pg.Position.SquaredDistanceFrom(this.Position) < PerceptionDistance;
        }

        public override void Die(Character pg)
        {
            // Pg Xp, Gold, Items, etc...
            pg.GainExperience(AICharacter.XpCalculation(pg, this));
            // Remove from map
        }
    }
}