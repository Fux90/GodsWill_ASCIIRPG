using GodsWill_ASCIIRPG.Main;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public abstract class AI
	{
        public delegate Direction FindDirectionMethod(AICharacter me, Pg pg);
        public delegate Direction RandomDirectionChangeMethod(Direction currentDirection, int perc = 100);

        protected enum Status
        {
            Wandering,
            Chasing
        }

        private AICharacter controlledCharacter;
        public AICharacter ControlledCharacter
        {
            get
            {
                return controlledCharacter;
            }
            set
            {
                controlledCharacter = value;
            }
        }

        public AI()
        {
            FindDirection = DirectionFindingAlgorithms.SimpleChase;
            RandomDirectionChange = RandomDirectionChangeAlgorithms.AlwaysAhead;
        }

        public FindDirectionMethod FindDirection { get; protected set; }
        public RandomDirectionChangeMethod RandomDirectionChange { get; protected set; }

        public abstract void ExecuteAction();
	}

    static class DirectionFindingAlgorithms
    {
        public static AI.FindDirectionMethod SimpleChase = (me, pg) =>
        {
            return Direction.North;
        };
    }

    static class RandomDirectionChangeAlgorithms
    {
        public static AI.RandomDirectionChangeMethod AlwaysAhead = (currDir, p) =>
        {
            return currDir;
        };

        public static AI.RandomDirectionChangeMethod RandomAtPerc = (currDir, paces) =>
        {
            return Dice.Throws(100) <= paces ? currDir.RandomDifferentFromThis() : currDir;
        };

        public static AI.RandomDirectionChangeMethod TurnBackAtPerc = (currDir, paces) =>
        {
            return Dice.Throws(100) <= paces ? currDir.TurnBack() : currDir;
        };
    }

    public class SimpleAI : AI
    {
        Status currentStatus;
        Direction currentDirection;

        public SimpleAI()
            : base ()
        {
            currentStatus = Status.Wandering;
            //FindDirection = DirectionFindingAlgorithms.SimpleChase;
            //RandomDirectionChange = RandomDirectionChangeAlgorithms.RandomAtPerc;
        }

        public override void ExecuteAction()
        {
            bool acted = false;

            switch(currentStatus)
            {
                case Status.Wandering:
                    // Movement in last direction
                    if(ControlledCharacter.SensePg(Game.Current.CurrentPg))
                    {
                        currentStatus = Status.Chasing;
                        ControlledCharacter.Talk();
                    }
                    currentDirection = RandomDirectionChange(currentDirection, 40);
                    if(!ControlledCharacter.Move(currentDirection, out acted))
                    {
                        // Change direction
                        currentDirection = currentDirection.RandomDifferentFromThis();
                    }
                    break;
                case Status.Chasing:
                    var direction = FindDirection(ControlledCharacter, Game.Current.CurrentPg);
                    ControlledCharacter.Move(direction, out acted);
                    break;
            }
        }
    }
}