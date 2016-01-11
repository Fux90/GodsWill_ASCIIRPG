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
            var lvlMinus1 = lvl - 1;

            Name = Name.ValueIfNotNullOrElse("Orc");
            Stats = Stats.ValueIfNotNullOrElse(new GodsWill_ASCIIRPG.Stats(
                                                    StatsBuilder.RandomStats(
                                                        new Dictionary<StatsType, int>()
                                                        {
                                                            { StatsType.Strength, 2},
                                                            { StatsType.Toughness, 2},
                                                            { StatsType.Mental, -2},
                                                            { StatsType.InnatePower, -2},
                                                        })));
            MaximumPf = CurrentPf.ValueIfNotNullOrElse(Orc.healthDice.Max
                                                       + Dice.Throws(Orc.healthDice, lvlMinus1) 
                                                       + lvl * ((Stats)Stats)[StatsType.Toughness]);
            CurrentPf = MaximumPf.ValueIfNotNullOrElse((int)CurrentPf);
            Hunger.ValueIfNotNullOrElse((Orc.hungerDice.Max
                                        + Dice.Throws(Orc.hungerDice, lvlMinus1))
                                        * ((Stats)Stats)[StatsType.Toughness]);
            MyAI = MyAI.ValueIfNotNullOrElse(new SimpleAI());
            PerceptionDistance = PerceptionDistance.ValueIfNotNullOrElse(5);
            //WornArmor - Can be null
            //EmbracedShield - Can be null
            //HandledWeapon - Can be null
            Backpack = Backpack.ValueIfNotNullOrElse(new Backpack());
            Symbol = Symbol.ValueIfNotNullOrElse("o");
            Color = Color.ValueIfNotNullOrElse(System.Drawing.Color.DarkOliveGreen);
            Description = Description.ValueIfNotNullOrElse("A human tall c");
            Position = Position.ValueIfNotNullOrElse(new Coord());
            Hostile = Hostile.ValueIfNotNullOrElse(true);

            var orc = new Orc(  Name,
                                (int)CurrentPf,
                                (int)MaximumPf,
                                (int)Hunger,
                                MyAI,
                                (int)PerceptionDistance,
                                (Stats)Stats,
                                WornArmor,
                                EmbracedShield,
                                HandledWeapon,
                                Backpack,
                                Symbol,
                                (System.Drawing.Color)Color,
                                Description,
                                (Coord)Position,
                                (bool)Hostile);

            return orc;
        }
    }

    public class Orc : AICharacter
    {
        public static readonly Dice healthDice = new Dice(nFaces: 8);
        public static readonly Dice hungerDice = new Dice(nFaces: 4);

        [Prerequisite(MinimumLevel = Pg.Level.Novice)]
        public Orc( string name,
                    int currentPf,
                    int maximumPf,
                    int hunger,
                    AI intelligence,
                    int perceptionDistance,
                    Stats stats,
                    Armor wornArmor,
                    Shield embracedShield,
                    Weapon handledWeapon,
                    Backpack backpack,
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
                    stats,
                    wornArmor,
                    embracedShield,
                    handledWeapon,
                    backpack,
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
    }
}
