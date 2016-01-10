using GodsWill_ASCIIRPG.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	abstract class AI
	{
        public delegate Direction FindDirectionMethod(AICharacter me, Pg pg);

        protected enum Status
        {
            Wandering,
            Chasing
        }

        private AICharacter controlledCharacter;
        protected AICharacter ControlledCharacter
        {
            get
            {
                return controlledCharacter;
            }
        }

        public AI(AICharacter character)
        {
            this.controlledCharacter = character;
        }

        public FindDirectionMethod FindDirection { get; protected set; }

        public abstract void ExecuteAction();
	}

    static class DirectionFindingAlgorithms
    {
        public static AI.FindDirectionMethod SimpleChase = (me, pg) =>
        {
            return Direction.North;
        };
    }

    class SimpleAI : AI
    {
        Status currentStatus;
        Direction currentDirection;

        public SimpleAI(AICharacter character)
            : base (character)
        {
            currentStatus = Status.Wandering;
            FindDirection = DirectionFindingAlgorithms.SimpleChase;
        }

        public override void ExecuteAction()
        {
            switch(currentStatus)
            {
                case Status.Wandering:
                    // Movement in last direction
                    if(ControlledCharacter.SensePg(Game.Current.CurrentPg))
                    {
                        currentStatus = Status.Chasing;
                    }
                    if(!ControlledCharacter.Move(currentDirection))
                    {
                        // Change direction
                    }
                    break;
                case Status.Chasing:
                    var direction = FindDirection(ControlledCharacter, Game.Current.CurrentPg);
                    ControlledCharacter.Move(direction);
                    break;
            }
        }
    }
}