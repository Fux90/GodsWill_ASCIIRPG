using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GodsWill_ASCIIRPG
{
    class MapBuilder
    {
        #region PRIVATE_MEMBERS
        List<IMapViewer> views;
        List<Atom> elements;
        #endregion

        #region PROPERTIES
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Coord PlayerInitialPosition { get; set; }
        #endregion

        public MapBuilder()
        {
            Name = "Map";
            Width = 1;
            Height = 1;
            PlayerInitialPosition = new Coord() { X = 1, Y = 1 };
            views = new List<IMapViewer>();
            elements = new List<Atom>();
        }

        public Map LoadFromFile(string mapFilePath)
        {
            var formatter = new BinaryFormatter();
            FileStream mapFile = new FileStream(mapFilePath, FileMode.Open);
            return (Map)formatter.Deserialize(mapFile);
        } 

        public void AddViewer(IMapViewer viewer)
        {
            views.Add(viewer);
        }

        public void AddAtom(Atom a)
        {
            elements.Add(a);
        }

        public Map Create()
        {
            var table = new BidimensionalArray<Atom>(Height, Width);
            
            for (int r = 0; r < table.Rows; r++)
            {
                for (int c = 0; c < table.Cols; c++)
                {
                    table[r, c] = new Floor(new Coord() { X = c, Y = r });
                }
            }
            var map = new Map(Name, PlayerInitialPosition, table);
            for (int i = 0; i < views.Count; i++)
            {
                map.RegisterViewer(views[i]);
            }
            foreach (var atom in elements)
            {
                //map.Insert(atom);
                atom.InsertInMap(map, atom.Position);
            }

            return map;
        }
    }

    [Serializable]
    public class Map : ISerializable
    {
        #region SERIALIZATION_CONST_NAMES
        private const string nameSerializableName = "name";
        private const string tableSerializableName = "table";
        private const string bufferSerializableName = "buffer";
        private const string viewsSerializableName = "views";
        #endregion

        #region PRIVATE_MEMBERS
        string name;
        Coord playerInitialPosition;
        BidimensionalArray<Atom> table;
        BidimensionalArray<Atom> buffer;
        BidimensionalArray<bool> explored;
        List<IMapViewer> views;
        #endregion

        #region PROPERTIES
        public string Name { get { return name; } }
        public int Width { get { return table.Width; } }
        public int Height { get { return table.Height; } }
        public Coord PlayerInitialPosition { get { return playerInitialPosition; } }
        #endregion

        #region ITERATORS
        public Atom this[Coord coord]
        {
            get
            {
                return this.table[coord];
            }
        }
        #endregion

        public Map( string name,
                    Coord playerInitialPosition,
                    BidimensionalArray<Atom> table)
        {
            this.name = name;
            this.playerInitialPosition = playerInitialPosition;
            this.table = table;
            this.buffer = new BidimensionalArray<Atom>(table.Rows, table.Cols);
            this.views = new List<IMapViewer>();
        }

        #region METHODS

        public void RegisterViewer(IMapViewer viewer)
        {
            views.Add(viewer);
        }

        public void NotifyViewersOfMovement(Coord freedCell, Coord occupiedCell)
        {
            foreach (var viewer in views)
            {
                viewer.NotifyMovement(freedCell, occupiedCell);
            }
        }

        public void NotifyViewersOfRemoval(Coord freedCell)
        {
            foreach (var viewer in views)
            {
                viewer.NotifyRemoval(freedCell);
            }
        }

        public void Insert(Atom a)
        {
            if (CanMoveTo(a.Position))
            {
                buffer[a.Position] = table[a.Position];
                table[a.Position] = a;
            }
        }

        public void Remove(Atom a)
        {
            RemoveAt(a.Position);
        }

        public void RemoveAt(Coord coord)
        {
            table[coord] = buffer[coord];
            buffer[coord] = new Floor(coord);
            NotifyViewersOfRemoval(coord);
        }

        public void RemoveFromBuffer(Atom a)
        {
            RemoveFromBufferAt(a.Position);
        }

        public void RemoveFromBufferAt(Coord coord)
        {
            buffer[coord] = new Floor(coord);
        }

        public Atom UnderneathAtom(Coord pos)
        {
            return buffer[pos];
        }

        public void RemoveUnderneathAtom(Coord pos)
        {
            buffer[pos] = new Floor(pos);
        }

        public bool CanMoveTo(Coord coord)
        {
            return table[coord.X, coord.Y].Walkable;
        }

        public void MoveAtomTo(Atom movedAtom, Coord prevPosition, Coord newPosition)
        {
            table[prevPosition] = buffer[prevPosition];
            buffer[newPosition] = table[newPosition];
            table[newPosition] = movedAtom;
            NotifyViewersOfMovement(prevPosition, newPosition);
            var steppedAtom = buffer[newPosition];
            if (steppedAtom.GetType().IsSubclassOf(typeof(Item)))
            {
                var item = ((Item)steppedAtom);
                steppedAtom.NotifyListeners(item.ItemTypeName);
                if (movedAtom.GetType().IsSubclassOf(typeof(Pg)))
                {
                    movedAtom.NotifyListeners(String.Format("Stepped on {0}", item.ItemTypeName));
                }
            }
        }

        #endregion

        #region SERIALIZATION
        public Map(SerializationInfo info, StreamingContext context)
        {
            name = (string)info.GetValue(nameSerializableName, typeof(string));
            table = (BidimensionalArray<Atom>)info.GetValue(tableSerializableName, typeof(BidimensionalArray<Atom>));
            buffer = (BidimensionalArray<Atom>)info.GetValue(bufferSerializableName, typeof(BidimensionalArray<Atom>));
            views = (List<IMapViewer>)info.GetValue(viewsSerializableName, typeof(List<IMapViewer>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameSerializableName, name, typeof(string));
            info.AddValue(tableSerializableName, table, typeof(BidimensionalArray<Atom>));
            info.AddValue(bufferSerializableName, buffer, typeof(BidimensionalArray<Atom>));
            info.AddValue(viewsSerializableName, views, typeof(List<IMapViewer>));
        }
        #endregion
    }
}