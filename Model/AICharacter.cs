using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.AICharacters;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
    public abstract class AICharacterBuilder 
    {
        public string Name { get; set; }
        public God God { get; set; }
        public int? CurrentPf { get; set; }
        public int? MaximumPf { get; set; }
        public int? Hunger { get; set; }
        public AI MyAI { get; set; }
        public AI.SensingMethod MySensingMethod { get; set; }
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
        public Allied? AlliedTo { get; set; }

        public AICharacterBuilder() { }

        protected abstract AICharacter RandomBuild(Pg.Level level);

        public virtual AICharacter Build(Pg.Level level = Pg.Level.Novice)
        {
            var aiChar = RandomBuild(level);
            return aiChar;
        }
    }

    [Serializable]
	public abstract class AICharacter : Character, ISerializable, IXPGiveable
    {
        #region SERIALIZATION_CONST_NAMES
        const string hostileSerializationName = "hostile";
        const string aiTypeSerializationName = "type";
        const string aiSerializationName = "ai";
        const string sensingSerializationName = "sensing";
        const string perceptionDistanceSerializationName = "perceptionDist";
        const string squaredPerceptionDistanceSerializationName = "sqrPerceptionDist";
        #endregion

        private AI intelligence;
        
        public bool Hostile { get { return this.AlliedTo == Allied.Enemy; } }
        public AI AI { get { return intelligence; } }
        private int perceptionDistance;
        private int squaredPerceptionDistance;
        public int PerceptionDistance
        {
            get { return perceptionDistance; }
            protected set
            {
                perceptionDistance = value;
                squaredPerceptionDistance = perceptionDistance * perceptionDistance;
            }
        }
        public int SquaredPerceptionDistance { get { return squaredPerceptionDistance; } }
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
                    null,
                    true,
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
                            AI.SensingMethod sensingMethod = null,
                            Stats? stats = null,
                            Armor wornArmor = null,
                            Shield embracedShield = null,
                            Weapon handledWeapon = null,
                            Backpack backpack = null,
                            God god = null,
                            bool unblockable = false,
                            string symbol = "C",
                            Color? color = null,
                            string description = "A creature of the world", 
                            Coord position = new Coord(),
                            Allied hostile = Allied.Enemy)
            : base( name, 
                    currentPf,
                    maximumPf,
                    hunger,
                    stats == null ? new Stats(StatsBuilder.RandomStats()) : (Stats)stats,
                    wornArmor,
                    embracedShield, 
                    handledWeapon, 
                    backpack == null ? new Backpack() : backpack,
                    god,
                    unblockable,
                    symbol,
                    color == null 
                        ? (hostile == Allied.Enemy ? Color.Red : Color.Green)
                        : (Color)color,
                    description,
                    position)
        {
            this.intelligence = intelligence;
            this.SensePg = sensingMethod == null ? AI.SensingAlgorythms.AllAround : sensingMethod;
            this.intelligence.ControlledCharacter = this;
            this.PerceptionDistance = perceptionDistance;
            this.AlliedTo = hostile;
        }

        public AICharacter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var typeName = (string)info.GetValue(aiTypeSerializationName, typeof(string));
            var type = Type.GetType(typeName);
            intelligence = (AI)Activator.CreateInstance(type, new object[] { info, context});
            this.intelligence.ControlledCharacter = this;
            var senseMethodName = (string)info.GetValue(sensingSerializationName, typeof(string));
            SensePg = senseMethodName.ToDelegate<AI.SensingAlgorythms, AI.SensingMethod>();
            perceptionDistance = (int)info.GetValue(perceptionDistanceSerializationName, typeof(int));
            squaredPerceptionDistance =  (int)info.GetValue(squaredPerceptionDistanceSerializationName, typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(aiTypeSerializationName, intelligence.GetType().FullName, typeof(string));
            //This way doesn't work
            //info.AddValue(aiSerializationName, intelligence.ToString(), typeof(string));
            intelligence.GetObjectData(info, context);
            info.AddValue(  sensingSerializationName,
                            SensePg.Method.Name,
                            typeof(string));
            info.AddValue(perceptionDistanceSerializationName, perceptionDistance, typeof(int));
            info.AddValue(squaredPerceptionDistanceSerializationName, squaredPerceptionDistance, typeof(int));
        }

        public override bool Interaction(Atom interactor)
        {
            var interactorType = interactor.GetType();
            if (interactorType == typeof(Pg) && Hostile)
            {
                var pg = interactor as Pg;
                pg.Attack(this);
                
                return true;
            }
            else if(interactorType.IsSubclassOf(typeof(AICharacter)))
            {
                var aiChar = interactor as AICharacter;
                if(aiChar.AlliedTo != this.AlliedTo 
                    && aiChar.AlliedTo != Allied.None
                    && this.AlliedTo != Allied.None)
                {
                    aiChar.Attack(this);
                }
                else
                {
                    // 
                }
            }

            return false;
        }

        public AI.SensingMethod SensePg { get; protected set; }

        public int XPPremium
        {
            get
            {
                //var attributes = (XPPremium[])this.GetType().GetCustomAttributes(typeof(XPPremium), false);
                var attributes = this.Attributes(typeof(XPPremium), false);
                return attributes.Count == 0
                        ? 0
                        : ((XPPremium)attributes[0]).Value;
            }
        }

        public override void Die(IFighter pg)
        {
            var killerType = pg.GetType();
            var pgType = typeof(Pg);

            if (killerType == pgType || killerType.IsSubclassOf(pgType))
            {
                var charPg = pg as Pg;
                // Pg Xp, Gold, Items, etc...
                charPg.GainExperience(this.XPPremium);
                // Remove from map
            }
            this.Map.Remove(this);
        }

        public virtual void Talk()
        {
            NotifyListeners("*bla bla bla*");
        }
    }
}