using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG
{
    class MapBuilder
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Coord PlayerInitialPosition { get; set; }
        List<IMapViewer> views;
        List<Atom> elements;

        public MapBuilder()
        {
            Name = "Map";
            Width = 1;
            Height = 1;
            PlayerInitialPosition = new Coord() { X = 1, Y = 1 };
            views = new List<IMapViewer>();
            elements = new List<Atom>();
        }

        public void LoadFromFile(string mapFilePath)
        {
            //throw new NotImplementedException();
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
                map.Insert(atom);
            }

            return map;
        }
    }

    public class Map
    {
        string name;
        Coord playerInitialPosition;
        BidimensionalArray<Atom> table;
        BidimensionalArray<Atom> buffer;
        List<IMapViewer> views;

        public string Name { get { return name; } }
        public int Width { get { return table.Width; } }
        public int Height { get { return table.Height; } }
        public Coord PlayerInitialPosition { get { return playerInitialPosition; } }

        public Atom this[Coord coord]
        {
            get
            {
                return this.table[coord];
            }
        }

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

        public void RegisterViewer(IMapViewer viewer)
        {
            views.Add(viewer);
        }

        /// <summary>
        /// Da definire...
        /// </summary>
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

        internal void MoveAtomTo(Atom movedAtom, Coord prevPosition, Coord newPosition)
        {
            table[prevPosition] = buffer[prevPosition];
            buffer[newPosition] = table[newPosition];
            table[newPosition] = movedAtom;
            NotifyViewersOfMovement(prevPosition, newPosition);
        }
    }
}