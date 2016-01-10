using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
	class AICharacter : Character
	{
        public delegate int XPCalculationMethod(Character pg, Character monster);

        private bool hostile;
        private AI intelligence;
        
        public bool Hostile { get { return hostile; } }
        public AI Intelligence { get { return intelligence; } }
        public int PerceptionDistance { get; protected set; }

        public AICharacter(AI intelligence, int perceptionDistance, bool hostile = true)
            : base("AI Character", 10, 10, 10, new Stats(StatsBuilder.RandomStats()), null, null, null, new Backpack())
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