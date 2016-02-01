using GodsWill_ASCIIRPG.Main;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
	public abstract class AI : ISerializable
	{
        #region SERIALIZABLE_CONST
        const string findDirectionSerializableName = "findDirection";
        const string randomDirectionChangeSerializableName = "randomDirection";
        #endregion

        public class DirectionFindingAlgorithms
        {
            public static AI.FindDirectionMethod SimpleChase
            {
                get
                {
                    return (me, pg) =>
                    {
                        var firstStepPos = new Line(me.Position, pg.Position)[1];
                        return me.Position.DirectionFromOffset(firstStepPos);
                    };
                }
            }

            public static AI.FindDirectionMethod Stupid
            {
                get
                {
                    return (me, pg) => Direction.North;
                }
            }
        }

        public class RandomDirectionChangeAlgorithms
        {
            public static AI.RandomDirectionChangeMethod AlwaysAhead
            {
                get
                {
                    return (currDir, p) =>
                    {
                        return currDir;
                    };
                }
            }

            public static AI.RandomDirectionChangeMethod RandomAtPerc
            {
                get
                {
                    return (currDir, paces) =>
                    {
                        return Dice.Throws(100) <= paces ? currDir.RandomDifferentFromThis() : currDir;
                    };
                }
            }

            public static AI.RandomDirectionChangeMethod TurnBackAtPerc
            {
                get
                {
                    return (currDir, paces) =>
                    {
                        return Dice.Throws(100) <= paces ? currDir.Opposite() : currDir;
                    };
                }
            }
        }

        public class SensingAlgorythms
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

        public AI(SerializationInfo info, StreamingContext context)
        {
            FindDirection = DirectionFindingAlgorithms.SimpleChase;
            RandomDirectionChange = RandomDirectionChangeAlgorithms.AlwaysAhead;

            var fdName = (string)info.GetValue(findDirectionSerializableName, typeof(string));
            //var m1 = typeof(DirectionFindingAlgorithms).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            //FindDirection = m1.Where(mi => ((FindDirectionMethod)mi.GetValue(mi)).Method.Name == fdName)
            //                .Select( mI => ((FindDirectionMethod)mI.GetValue(mI))).Single();
            FindDirection = fdName.ToDelegate<DirectionFindingAlgorithms, FindDirectionMethod>();

            //RandomDirectionChange = (RandomDirectionChangeMethod)info.GetValue(randomDirectionChangeSerializableName, typeof(RandomDirectionChangeMethod));
            var rdName = (string)info.GetValue(randomDirectionChangeSerializableName, typeof(string));
            //var m2 = typeof(RandomDirectionChangeAlgorithms).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            //RandomDirectionChange = m2.Where(mi => ((RandomDirectionChangeMethod)mi.GetValue(mi)).Method.Name == rdName)
            //                        .Select(mI => ((RandomDirectionChangeMethod)mI.GetValue(mI))).Single();
            RandomDirectionChange = rdName.ToDelegate<RandomDirectionChangeAlgorithms, RandomDirectionChangeMethod>();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(  findDirectionSerializableName, 
                            FindDirection.Method.Name, 
                            typeof(string));
            info.AddValue(  randomDirectionChangeSerializableName, 
                            RandomDirectionChange.Method.Name, 
                            typeof(string));
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

    [Serializable]
    public class SimpleAI : AI, ISerializable
    {
        #region SERIALIZABLE_CONST
        const string currentStatusSerializableName = "currStatus";
        const string currentDirectionSerializableName = "currDirection";
        #endregion

        Status currentStatus;
        Direction currentDirection;

        public SimpleAI()
            : base ()
        {
            currentStatus = Status.Wandering;
            RandomDirectionChange = RandomDirectionChangeAlgorithms.RandomAtPerc;
        }

        public SimpleAI(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            currentStatus = (Status)info.GetValue(currentStatusSerializableName, typeof(Status));
            currentDirection = (Direction)info.GetValue(currentDirectionSerializableName, typeof(Direction));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(currentStatusSerializableName, currentStatus, typeof(Status));
            info.AddValue(currentDirectionSerializableName, currentDirection, typeof(Direction));
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