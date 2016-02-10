using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.View;
using System.ComponentModel;
using System.Runtime.Serialization;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
    abstract public class Atom : ISerializable, IViewable<IAtomListener>
    {
        #region SERIALIZATION_CONST_NAMES
        private const string nameSerializableName = "name";
        private const string symbolSerializableName = "symbol";
        private const string colorSerializableName = "color";
        private const string walkableSerializableName = "walkable";
        private const string descriptionSerializableName = "description";
        private const string positionSerializableName = "position";
        private const string mapSerializableName = "map";
        private const string listenersSerializableName = "listeners";
        private const string isPickableSerializableName = "isPickable";
        #endregion

        #region PRIVATE_MEMEBERS
        private string name;
        private string symbol;
        private Color color;
        private bool walkable;
        private bool blockVision;
        private string description;
        private Coord position;
        private Map map;
        private List<IAtomListener> listeners;
        #endregion

        #region PROPERTIES
        public virtual string Name { get { return name; } }
        public virtual string Symbol { get { return symbol; } }
        public virtual Color Color { get { return color; } }
        public bool Walkable { get { return walkable; } }
        public bool BlockVision { get { return blockVision; } }
        public string Description { get { return description; } }
        public Coord Position { get { return position; } protected set { position = value; } }
        public Map Map { get { return map; } }
        public bool IsPickable { get; protected set; }
        public List<IAtomListener> Listeners { get { return new List<IAtomListener>(listeners); } }
        public bool Physical { get { return this.GetType().GetCustomAttributes(typeof(Unphysical), true).Length == 0; } }
        public bool Unphysical { get { return !Physical; } }
        #endregion

        public Atom(string name,
                    string symbol,
                    Color color,
                    bool walkable,
                    bool blockVision,
                    string description = "Base element of the game",
                    Coord position = new Coord())
        {
            this.name = name;
            this.symbol = symbol;
            this.color = color;
            this.walkable = walkable;
            this.blockVision = blockVision;
            this.description = description;
            this.position = position;
            this.listeners = new List<IAtomListener>();
        }

        public Atom(SerializationInfo info, StreamingContext context)
        {
            name = (string)info.GetValue(nameSerializableName, typeof(string));
            symbol = (string)info.GetValue(symbolSerializableName, typeof(string));
            color = (Color)info.GetValue(colorSerializableName, typeof(Color));
            walkable = (bool)info.GetValue(walkableSerializableName, typeof(bool));
            description = (string)info.GetValue(descriptionSerializableName, typeof(string));
            position = (Coord)info.GetValue(positionSerializableName, typeof(Coord));
            //map = (Map)info.GetValue(mapSerializableName, typeof(Map));
            //listeners = (List<IAtomListener>)info.GetValue(listenersSerializableName, typeof(List<IAtomListener>));
            this.listeners = new List<IAtomListener>();
            IsPickable = (bool)info.GetValue(isPickableSerializableName, typeof(bool));
        }

        #region METHODS
        public virtual void InsertInMap(Map map, Coord newPos, bool overwrite = false)
        {
            this.map = map;
            this.position = newPos;
            var type = this.GetType();
            var typePg = typeof(Pg);
            if (type == typePg || type.IsSubclassOf(typePg))
            {
                NotifyListeners(String.Format("Entered {0}", map.Name));
            }
            this.map.Insert(this, overwrite);
        }

        public void NotifyListeners(string msg)
        {
            foreach (var listener in listeners)
            {
                listener.NotifyMessage(this, msg);
            }
        }

        public void RegisterView(IAtomListener listener)
        {
            listeners.AddOnce(listener);
        }

        public void UnregisterListener(IAtomListener listener)
        {
            listeners.Remove(listener);
        }

        public abstract bool Interaction(Atom interactor);
        #endregion

        #region SERIALIZATION

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameSerializableName, name, typeof(string));
            info.AddValue(symbolSerializableName, symbol, typeof(string));
            info.AddValue(colorSerializableName, color, typeof(Color));
            info.AddValue(walkableSerializableName, walkable, typeof(bool));
            info.AddValue(descriptionSerializableName, description, typeof(string));
            info.AddValue(positionSerializableName, position, typeof(Coord));
            //info.AddValue(mapSerializableName, map, typeof(Map));
            //info.AddValue(listenersSerializableName, listeners, typeof(List<IAtomListener>));
            info.AddValue(isPickableSerializableName, IsPickable, typeof(bool));
        }

        public bool HasToBeInStraightSight
        {
            get
            {
                return this.GetType().GetCustomAttributes(typeof(StraightSightNeededForPerception), true).Length > 0;
            }
        }
        #endregion

        #region REQUISITES
        public virtual bool SatisfyRequisite(Pg player)
        {
            var satisfied = true;

            var requisites = this.GetType().GetCustomAttributes(typeof(Prerequisite), true);
            if(requisites.Length > 0)
            {
                var r = (Prerequisite)requisites[0];

                satisfied &= player.CurrentLevel >= r.MinimumLevel;
                satisfied &= player.Stats >= r.MinimumStats;
            }
            
            return satisfied;
        }

        public void SetMap(Map map)
        {
            this.map = map;
        }
        #endregion
    }

    [Serializable]
    abstract public class HiddenAtom : Atom, ISerializable
    {
        private const int baseCDPerception = 10;
        private const string hiddenSerializationName = "hidden";

        public bool Hidden { get; private set; }
        public override string Symbol
        {
            get
            {
                return Hidden ? Floor._Symbol : base.Symbol;
            }
        }

        public override Color Color
        {
            get
            {
                return Hidden ? Floor._Color : base.Color;
            }
        }

        public HiddenAtom(  string name,
                            string symbol,
                            Color color,
                            bool walkable,
                            bool blockVision,
                            string description = "Element of the game initially hidden",
                            Coord position = new Coord(),
                            bool hidden = true)
            : base( name, 
                    symbol,
                    color,
                    walkable,
                    blockVision,
                    description,
                    position)
        {
            Hidden = hidden;
        }

        public HiddenAtom(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Hidden = (bool)info.GetValue(hiddenSerializationName, typeof(bool));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(hiddenSerializationName, Hidden, typeof(bool));
        }

        public void NotifyIndividuation()
        {
            Hidden = false;
            NotifyListeners("Detected");
        }

        protected void Show()
        {
            Hidden = false;
        }
    }

    [Serializable]
    public abstract class MoveableAtom : Atom
    {
        public bool Unblockable { get; private set; }

        public MoveableAtom(string name,
                            string symbol,
                            Color color,
                            bool walkable,
                            bool blockVision,
                            bool unblockable,
                            string description = "Base moveable element of the game",
                            Coord position = new Coord())
            : base( name,
                    symbol,
                    color,
                    walkable,
                    blockVision,
                    description,
                    position)
        {
            Unblockable = unblockable;
        }

        public MoveableAtom(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public virtual bool Move(Direction dir, out bool acted)
        {
            var candidateCoord = new Coord()
            {
                X = this.Position.X,
                Y = this.Position.Y
            };

            switch (dir)
            {
                case Direction.North:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    break;
                case Direction.NorthEast:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.East:
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.SouthEast:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.South:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    break;
                case Direction.SouthWest:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
                case Direction.West:
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
                case Direction.NorthWest:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
            }

            var moved = false;
            acted = false;
            Atom steppedOnto = this.Map[candidateCoord];

            if (this.Unblockable
                || this.Map.CanMoveTo(candidateCoord)
                || this.IsNotBlockedFromType(steppedOnto))
            {
                bool immediate = CanHappenTriggerEvent(this, steppedOnto);

                this.Map.MoveAtomTo(this, this.Position, candidateCoord);
                this.Position = candidateCoord;

                moved = true;
                acted = true;

                if(immediate)
                {
                    ((ITriggerActor)this).TriggerCurrent();
                }
            }
            else
            {
                acted = this.Map[candidateCoord].Interaction(this);
            }

            return moved;
        }

        public virtual bool IsNotBlockedFromType(Atom atom)
        {
            //var atomType = ...
            return false;
        }

        public virtual bool CanHappenTriggerEvent(  MoveableAtom triggerer, 
                                                    Atom triggerable)
        {
            var immediate = false;

            var typeOfStepped = triggerable.GetType();

            if (typeof(ITriggerable).IsAssignableFrom(triggerable.GetType()))
            {
                if (typeof(ITriggerActor).IsAssignableFrom(triggerer.GetType()))
                {
                    var _triggerable = (ITriggerable)triggerable;
                    var _triggerer = (ITriggerActor)triggerer;

                    _triggerer.RegisterTriggerable(_triggerable);
                    _triggerable.RegisterTriggeringSubject(triggerer);

                    immediate = _triggerable.ImmediateTrigger;
                }
            }

            return immediate;
        }
    }

    [Serializable]
    abstract public class TwoLevelNotifyableAtom : Atom,
        ISerializable, 
        IViewable<IAtomListener>, ITwoLevelViewable<IAtomListener, string>
    {
        private List<IAtomListener> secondaryListeners;
        private List<IAtomListener> SecondaryListeners
        {
            get
            {
                if (secondaryListeners == null)
                {
                    secondaryListeners = new List<IAtomListener>();
                }

                return secondaryListeners;
            }
        }

        public TwoLevelNotifyableAtom(string name,
                    string symbol,
                    Color color,
                    bool walkable,
                    bool blockVision,
                    string description = "Base element of the game",
                    Coord position = new Coord())
            : base( name,
                    symbol,
                    color,
                    walkable,
                    blockVision,
                    description,
                    position)
        {
            
        }

        public TwoLevelNotifyableAtom(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        public void RegisterSecondaryView(IAtomListener viewer)
        {
            SecondaryListeners.AddOnce(viewer);
        }

        public void NotifySecondaryViewers(string content)
        {
            SecondaryListeners.ForEach(l => l.NotifyMessage(this, content));
        }
    }
}