using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.View;
using System.ComponentModel;

namespace GodsWill_ASCIIRPG
{
	abstract public class Atom
	{
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

        public void InsertInMap(Map map, Coord newPos)
        {
            this.map = map;
            this.position = newPos;
            NotifyListeners(String.Format("Entered {0}", map.Name));
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
	}
}