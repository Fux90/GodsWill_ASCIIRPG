using GodsWill_ASCIIRPG.Main;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{


	public abstract class AI
	{
        public static class DirectionFindingAlgorithms
        {
            public static AI.FindDirectionMethod SimpleChase = (me, pg) =>
            {
                var firstStepPos = new Line(me.Position, pg.Position)[1];
                return me.Position.DirectionFromOffset(firstStepPos);
            };
        }

        public static class RandomDirectionChangeAlgorithms
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
                return Dice.Throws(100) <= paces ? currDir.Opposite() : currDir;
            };
        }

        public static class SensingAlgorythms
        {
            public static AI.SensingMethod AllAround
            {
                get
                {
                    return (me, other) => other.Position.SquaredDistanceFrom(me.Position) < me.SquaredPerceptionDistance;
                }
            }

            public static AI.SensingMethod LineOfSight
            {
                get
                {
                    return (me, other) =>
                    {
                        if (other.Position.SquaredDistanceFrom(me.Position) < me.SquaredPerceptionDistance)
                        {
                            var sightLine = new Line(me.Position, other.Position);
                            foreach (Coord pt in sightLine)
                            {
                                if (me.Map[pt].BlockVision) return false;
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
            }
        }

        public delegate Direction FindDirectionMethod(AICharacter me, Pg pg);
        public delegate Direction RandomDirectionChangeMethod(Direction currentDirection, int perc = 100);
        public delegate bool SensingMethod(AICharacter me, Character otherCharacter);

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
        
        public abstract void ActionDescription(out bool moved, out bool acted);
        public void ExecuteAction()
        {
            bool acted;
            bool moved;

            ActionDescription(out moved, out acted);

            if (acted)
            {
                ControlledCharacter.EffectOfTurn();
            }
        }
    }

    public class SimpleAI : AI
    {
        Status currentStatus;
        Direction currentDirection;

        public SimpleAI()
            : base ()
        {
            currentStatus = Status.Wandering;
            RandomDirectionChange = RandomDirectionChangeAlgorithms.RandomAtPerc;
        }

        public override void ActionDescription( out bool moved,
                                                out bool acted)
        {
            moved = false;
            acted = false;

            switch(currentStatus)
            {
                case Status.Wandering:
                    // Movement in last direction
                    if(ControlledCharacter.SensePg(ControlledCharacter, Game.Current.CurrentPg))
                    {
                        currentStatus = Status.Chasing;
                        ControlledCharacter.Talk();
                    }
                    currentDirection = RandomDirectionChange(currentDirection, 5);
                    if(!(moved = ControlledCharacter.Move(currentDirection, out acted)))
                    {
                        // Change direction
                        currentDirection = currentDirection.RandomDifferentFromThis();
                        ControlledCharacter.Move(currentDirection, out acted);
                    }
                    break;
                case Status.Chasing:
                    var direction = FindDirection(ControlledCharacter, Game.Current.CurrentPg);
                    moved = ControlledCharacter.Move(direction, out acted);
                    break;
            }
        }
    }
}