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
        public string Description { get { return description; } }
        public Coord Position { get { return position; } protected set { position = value; } }
        public Map Map { get { return map; } }
        public bool IsPickable { get; protected set; }
        public List<IAtomListener> Listeners { get { return new List<IAtomListener>( listeners ); } }
        #endregion

        public Atom(string name,
                    string symbol,
                    Color color,
                    bool walkable,
                    string description = "Base element of the game",
                    Coord position = new Coord())
        {
            this.name = name;
            this.symbol = symbol;
            this.color = color;
            this.walkable = walkable;
            this.description = description;
            this.position = position;
            this.listeners = new List<IAtomListener>();
        }

        #region METHODS
        public void InsertInMap(Map map, Coord newPos)
        {
            this.map = map;
            this.position = newPos;
            var type = this.GetType();
            var typePg = typeof(Pg);
            if (type == typePg || type.IsSubclassOf(typePg))
            {
                NotifyListeners(String.Format("Entered {0}", map.Name));
            }
            this.map.Insert(this);
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

        public abstract void Interaction(Atom interactor);
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
            listeners = (List<IAtomListener>)info.GetValue(listenersSerializableName, typeof(List<IAtomListener>));
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
            info.AddValue(listenersSerializableName, listeners, typeof(List<IAtomListener>));
            info.AddValue(isPickableSerializableName, IsPickable, typeof(bool));
        }
        #endregion
    }
}