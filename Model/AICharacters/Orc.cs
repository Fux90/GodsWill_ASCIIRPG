using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodsWill_ASCIIRPG.Model.Core;
using System.Drawing;

namespace GodsWill_ASCIIRPG.Model.AICharacters
{
    public class OrcBuilder : AICharacterBuilder
    {
        protected override AICharacter RandomBuild(Pg.Level level)
        {
            var lvl = ((int)level);

            Name = Name.ValueIfNotNullOrElse("Orc");
            // God - can be null
            Stats = Stats.ValueIfNotNullOrElse(new GodsWill_ASCIIRPG.Stats(
                                                    StatsBuilder.RandomStats(
                                                        new Dictionary<StatsType, int>()
                                                        {
                                                            { StatsType.Strength, 2},
                                                            { StatsType.Toughness, 2},
                                                            { StatsType.Mental, -2},
                                                            { StatsType.InnatePower, -2},
                                                        })));
            var toughMod = ((Stats)Stats)[StatsType.Toughness].ModifierOfStat();
            MaximumPf = CurrentPf.ValueIfNotNullOrElse(Orc.healthDice.Max
                                                       + Dice.Throws(Orc.healthDice, lvl) 
                                                       + lvl * toughMod);
            CurrentPf = MaximumPf.ValueIfNotNullOrElse((int)MaximumPf);
            Hunger = Hunger.ValueIfNotNullOrElse((Orc.hungerDice.Max
                                        + Dice.Throws(Orc.hungerDice, lvl))
                                        * toughMod);
            MyAI = MyAI.ValueIfNotNullOrElse(new SimpleAI());
            MySensingMethod = MySensingMethod.ValueIfNotNullOrElse(AI.SensingAlgorythms.AllAround);
            PerceptionDistance = PerceptionDistance.ValueIfNotNullOrElse(5);
            //WornArmor - Can be null
            //EmbracedShield - Can be null
            //HandledWeapon - Can be null
            Backpack = Backpack.ValueIfNotNullOrElse(new Backpack());
            Symbol = Symbol.ValueIfNotNullOrElse("o");
            Color = Color.ValueIfNotNullOrElse(System.Drawing.Color.DarkOliveGreen);
            Description = Description.ValueIfNotNullOrElse("A greenish, smelly human-like creature. Strong, but usually not very smart.");
            Position = Position.ValueIfNotNullOrElse(new Coord());
            Hostile = Hostile.ValueIfNotNullOrElse(true);

            var orc = new Orc(  Name,
                                (int)CurrentPf,
                                (int)MaximumPf,
                                (int)Hunger,
                                MyAI,
                                MySensingMethod,
                                (int)PerceptionDistance,
                                (Stats)Stats,
                                WornArmor,
                                EmbracedShield,
                                HandledWeapon,
                                Backpack,
                                God,
                                Symbol,
                                (System.Drawing.Color)Color,
                                Description,
                                (Coord)Position,
                                (bool)Hostile);

            return orc;
        }
    }

    [Prerequisite(MinimumLevel = Pg.Level.Novice)]
    public class Orc : AICharacter
    {
        public static readonly Dice healthDice = new Dice(nFaces: 8);
        public static readonly Dice hungerDice = new Dice(nFaces: 4);
        
        public Orc()
            : base()
        {

        }

        public Orc( string name,
                    int currentPf,
                    int maximumPf,
                    int hunger,
                    AI intelligence,
                    AI.SensingMethod sensingMethod,
                    int perceptionDistance,
                    Stats stats,
                    Armor wornArmor,
                    Shield embracedShield,
                    Weapon handledWeapon,
                    Backpack backpack,
                    God god,
                    string symbol,
                    Color color,
                    string description,
                    Coord position,
                    bool hostile)
            : base( name,
                    currentPf,
                    maximumPf,
                    hunger,
                    intelligence,
                    perceptionDistance,
                    sensingMethod,
                    stats,
                    wornArmor,
                    embracedShield,
                    handledWeapon,
                    backpack,
                    god,
                    symbol,
                    color,
                    description,
                    position,
                    hostile)
        {

        }

        public override Dice HealthDice
        {
            get { return healthDice; }
        }

        public override Dice HungerDice
        {
            get { return hungerDice; }
        }

        public override AICharacterBuilder Builder
        {
            get
            {
                return new OrcBuilder();
            }
        }

        public override void Talk()
        {
            var msgs = new string[]
            {
                "*ROAR*",
                "I KILL YOU",
                String.Format("{0} smash you", Name),
            };
            var msg = Dice.Throws(msgs.Length) - 1;
            
            NotifyListeners(msgs[msg]);
        }
    }
}
