using GodsWill_ASCIIRPG.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public abstract class AI
	{
        public delegate Direction FindDirectionMethod(AICharacter me, Pg pg);

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

    public class SimpleAI : AI
    {
        Status currentStatus;
        Direction currentDirection;

        public SimpleAI()
            : base ()
        {
            currentStatus = Status.Wandering;
            FindDirection = DirectionFindingAlgorithms.SimpleChase;
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
                    else if(!ControlledCharacter.Move(currentDirection, out acted))
                    {
                        // Change direction
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