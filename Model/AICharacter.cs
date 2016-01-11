using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
    public abstract class AICharacterBuilder 
    {
        public string Name { get; set; }
        public int? CurrentPf { get; set; }
        public int? MaximumPf { get; set; }
        public int? Hunger { get; set; }
        public AI MyAI { get; set; }
        public int? PerceptionDistance { get; set; }
        public Stats? Stats { get; set; }
        public Armor WornArmor { get; set; }
        public Shield EmbracedShield { get; set; }
        public Weapon HandledWeapon { get; set; }
        public Backpack Backpack { get; set; }
        public string Symbol { get; set; }
        public Color? Color { get; set; }
        public string Description { get; set; }
        public Coord? Position { get; set; }
        public bool? Hostile { get; set; }

        public AICharacterBuilder() { }

        protected abstract AICharacter RandomBuild(Pg.Level level);

        public virtual AICharacter Build(Pg.Level level = Pg.Level.Novice)
        {
            var aiChar = RandomBuild(level);
            return aiChar;
        }
    }

	public abstract class AICharacter : Character
	{
        public delegate int XPCalculationMethod(Character pg, Character monster);

        private bool hostile;
        private AI intelligence;
        
        public bool Hostile { get { return hostile; } }
        public AI Intelligence { get { return intelligence; } }
        public int PerceptionDistance { get; protected set; }

        /// <summary>
        /// Done in this way (and not static) to be forced to have such property
        /// With reflection I will query all classes of monster, 
        /// call Builder accordingly 
        /// </summary>
        public abstract AICharacterBuilder Builder { get; }
        /// <summary>
        /// To call Builder
        /// </summary>
        public static AICharacter DummyCharacter(Type type){ return (AICharacter)Activator.CreateInstance(type); }

        public AICharacter()
            : base("", 0, 0, 0, new Stats(),
                    null,
                    null,
                    null,
                    new Backpack(),
                    "",
                    Color.White,
                    "",
                    new Coord())
        { }

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
            this.intelligence.ControlledCharacter = this;
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
            this.Map.Remove(this);
        }
    }
}