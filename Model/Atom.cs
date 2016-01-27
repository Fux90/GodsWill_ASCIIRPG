using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.View;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
    abstract public class Atom : ISerializable
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
        public string Name { get { return name; } }
        public string Symbol { get { return symbol; } }
        public Color Color { get { return color; } }
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

        public void RegisterListener(IAtomListener listener)
        {
            listeners.Add(listener);
        }

        public abstract bool Interaction(Atom interactor);
        #endregion

        #region SERIALIZATION
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
            IsPickable = (bool)info.GetValue(isPickableSerializableName, typeof(bool));
        }

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
        #endregion
    }

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

            if (this.Unblockable
                || this.Map.CanMoveTo(candidateCoord)
                || this.IsNotBlockedFromType(this.Map[candidateCoord]))
            {
                this.Map.MoveAtomTo(this, this.Position, candidateCoord);
                this.Position = candidateCoord;

                moved = true;
                acted = true;
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
    }
}