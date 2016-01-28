#define DEBUG_PRINT_MAP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GodsWill_ASCIIRPG.View;
using System.Drawing;
using GodsWill_ASCIIRPG.Model.SceneryItems;

namespace GodsWill_ASCIIRPG
{
    class MapBuilder
    {
        #region ENUM

        public enum TableCreationMode
        {
            Empty,
            Random,
            FromFile
        }

        #endregion

        #region PRIVATE_MEMBERS
        List<IMapViewer> views;
        List<IAtomListener> singleMsgListeners;
        List<Atom> elements;
        #endregion

        #region PROPERTIES
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Coord PlayerInitialPosition { get; set; }
        public TernaryValue Explored { get; set; }
        public bool Lightened { get; set; }
        public TableCreationMode MapCreationMode { get; set; }
        #endregion

        public MapBuilder()
        {
            Name = "Map";
            Width = 1;
            Height = 1;
            PlayerInitialPosition = new Coord() { X = 1, Y = 1 };
            views = new List<IMapViewer>();
            singleMsgListeners = new List<IAtomListener>();
            elements = new List<Atom>();
        }

        public Map LoadFromFile(string mapFilePath)
        {
            if (File.Exists(mapFilePath))
            {
                var formatter = new BinaryFormatter();
                FileStream mapFile = new FileStream(mapFilePath, FileMode.Open);
                return (Map)formatter.Deserialize(mapFile);
            }

            return null;
        }

        public void AddViewer(IMapViewer viewer)
        {
            views.Add(viewer);
        }

        public void AddSingleMessageListener(IAtomListener singleMsgListener)
        {
            this.singleMsgListeners.Add(singleMsgListener);
        }

        public void AddAtom(Atom a)
        {
            elements.Add(a);
        }


        //def place_objects_all_rooms(con, map, objects):
        //    for room in rooms:
        //        place_objects(con, map, room, objects)

        //def place_objects(con, map, room, objects):

        //    num_monsters = libt.random_get_int(0,0,MAX_ROOM_MONSTERS)
        //    for i in range(num_monsters):
        //        x = libt.random_get_int(0, room.x1, room.x2)
        //        y = libt.random_get_int(0, room.y1, room.y2)

        //        if not tile.is_blocked(x, y, map, objects):
        //            if libt.random_get_int(0,0,100) < 80: #80% di possibilita di pupparsi un orco
        //                monster = Object(con, x, y,'o','Orc', libt.desaturated_green, blocks = True)
        //            else:
        //                monster = Object(con, x, y,'T','Troll', libt.darker_green, blocks = True)
        //            objects.append(monster)
        //            monster.insertInMap(map)

        private void createHorizontalTunnel(BidimensionalArray<Atom> table,
                                     int x1,
                                     int x2,
                                     int y)
        {
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            {
                AddAtom(new Floor(new Coord(x, y)));
            }
        }

        private void createVerticalTunnel(BidimensionalArray<Atom> table,
                                     int y1,
                                     int y2,
                                     int x)
        {
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
            {
                AddAtom(new Floor(new Coord(x, y)));
            }
        }

        private void createRoom(Rectangle room, BidimensionalArray<Atom> map)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
			{
                for (int y = room.Top + 1; y < room.Bottom; y++)
			    {
                    AddAtom(new Floor(new Coord(x, y)));
                }
			}
        }

        private void makeMap(  out BidimensionalArray<Atom> map,
                                int MAX_ROOMS = 20,
                                int ROOM_MIN_SIZE = 3,
                                int ROOM_MAX_SIZE = 10)
        {
            map = new BidimensionalArray<Atom>(Height, Width, () => new Wall());
            List<Rectangle> rooms = new List<Rectangle>(MAX_ROOMS);

            int num_rooms = 0;
            int maxMinusMin = ROOM_MAX_SIZE - ROOM_MIN_SIZE;

            for (int r = 0; r < MAX_ROOMS; r++)
            {
                //W e H random
                int w = Dice.Throws(maxMinusMin) + ROOM_MIN_SIZE - 1;
                int h = Dice.Throws(maxMinusMin) + ROOM_MIN_SIZE - 1;

                // Random position into map borders
                int x = Dice.Throws(Width - w - 1) - 1;
                int y = Dice.Throws(Height - h - 1) - 1;

                var new_room = new Rectangle(x, y, w, h);

                var failed = false;
                foreach (var other_room in rooms)
                {
                    if (new_room.IntersectsWith(other_room))
                    {
                        failed = true;
                        break;
                    }
                }

                if (!failed)
                {
                    createRoom(new_room, map);
                    var newCoord = new_room.Center();

                    if (num_rooms == 0)
                    {
                        PlayerInitialPosition = newCoord;
                    }
                    else
                    {
                        var prevCoord = rooms[num_rooms - 1].Center();

                        if (Dice.Throws(2) == 1)
                        {
                            createHorizontalTunnel(map, prevCoord.X, newCoord.X, prevCoord.Y);
                            createVerticalTunnel(map, prevCoord.Y, newCoord.Y, newCoord.X);
                        }
                        else
                        {
                            createVerticalTunnel(map, prevCoord.Y, newCoord.Y, newCoord.X);
                            createHorizontalTunnel(map, prevCoord.X, newCoord.X, prevCoord.Y);
                        }
                    }

                    rooms.Add(new_room);
                    num_rooms++;
                }
            }
        }

        private void createEmptyTable(out BidimensionalArray<Atom> table)
        {
            table = new BidimensionalArray<Atom>(Height, Width, (pos) => new Floor(pos));

            //for (int r = 0; r < table.Rows; r++)
            //{
            //    for (int c = 0; c < table.Cols; c++)
            //    {
            //        table[r, c] = new Floor(new Coord() { X = c, Y = r });

            //    }
            //}
        }

        public Map Create()
        {
            //var table = new BidimensionalArray<Atom>(Height, Width);
            BidimensionalArray<Atom> table;
            var explored = new BidimensionalArray<bool>(Height, Width);
            Map map = null;

            switch (MapCreationMode)
            {
                case TableCreationMode.FromFile:
                    map = LoadFromFile(@"currentLevel.map");
                    table = null;
                    break;
                case TableCreationMode.Random:
                    makeMap(out table);
                    break;
                case TableCreationMode.Empty:
                default:
                    createEmptyTable(out table);
                    break;
            }

            if (map == null)
            {
                map = new Map(Name, PlayerInitialPosition, table, Explored, !Lightened);

                foreach (var atom in elements)
                {
                    //map.Insert(atom);
                    if (singleMsgListeners != null && atom.SupportsSingleMsgListener())
                    {
                        singleMsgListeners.ForEach(l => atom.RegisterListener(l));
                    }
                    atom.InsertInMap(map, atom.Position, true);
                }
            }
            else
            {
                if (singleMsgListeners != null)
                {
                    map.EveryAtom().ForEach(atom =>
                    {
                        singleMsgListeners.ForEach(l => atom.RegisterListener(l));
                    });
                }
            }

            for (int i = 0; i < views.Count; i++)
            {
                map.RegisterViewer(views[i]);
            }
            
            

            return map;
        }
    }

    [Serializable]
    public class Map : ISerializable
    {
        public enum LevelType
        {
            Normal,
            Untangibles
        }

        #region SERIALIZATION_CONST_NAMES
        private const string nameSerializableName = "name";
        private const string tableSerializableName = "table";
        private const string bufferSerializableName = "buffer";
        private const string exploredSerializableName = "explored";
        private const string darkSerializableName = "dark";
        private const string untangiblesSerializableName = "untangibles";
        private const string viewsSerializableName = "views";
        #endregion

        #region PRIVATE_MEMBERS
        string name;
        Coord playerInitialPosition;
        BidimensionalArray<Atom> table;
        BidimensionalArray<AtomCollection> untangibles;
        BidimensionalArray<Atom> buffer;
        BidimensionalArray<TernaryValue> explored;
        BidimensionalArray<bool> dark;
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

        public Atom this[Coord coord, LevelType level]
        {
            get
            {
                switch(level)
                {
                    case LevelType.Untangibles:
                        return this.untangibles[coord];
                    case LevelType.Normal:
                    default:
                        return this.table[coord];
                }
            }
        }
        #endregion

        public Map( string name,
                    Coord playerInitialPosition,
                    BidimensionalArray<Atom> table,
                    TernaryValue notToExplore,
                    bool dark)
        {
            this.name = name;
            this.playerInitialPosition = playerInitialPosition;
            this.table = table;
            this.buffer = new BidimensionalArray<Atom>(table.Rows, table.Cols, (pos) => new Floor(pos));
            this.untangibles = new BidimensionalArray<AtomCollection>(table.Rows, table.Cols, () => new AtomCollection());
            this.explored = new BidimensionalArray<TernaryValue>(table.Rows, table.Cols, notToExplore);
            this.dark = new BidimensionalArray<bool>(table.Rows, table.Cols, dark);
            this.views = new List<IMapViewer>();

            table.ForEach(atom => atom.InsertInMap(this, atom.Position, true));
        }

        #region METHODS

        public void RegisterViewer(IMapViewer viewer)
        {
            views.Add(viewer);
        }

        public void NotifyViewersOfMovement(Atom movedAtom, Coord freedCell, Coord occupiedCell)
        {
            foreach (var viewer in views)
            {
                viewer.NotifyMovement(movedAtom, freedCell, occupiedCell);
            }
        }

        public void NotifyViewersOfRemoval(Coord freedCell)
        {
            foreach (var viewer in views)
            {
                viewer.NotifyRemoval(freedCell);
            }
        }

        public void NotifyViewersOfExploration()
        {
            views.ForEach(view => view.NotifyExploration());
        }

        public bool IsCellExplored(Coord pos)
        {
            return explored[pos].ToBool();
        }

        public bool IsCellUnknown(Coord pos)
        {
            return explored[pos] == TernaryValue.Unknown;
        }

        public bool IsFullyExplored(Coord pos)
        {
            return explored[pos] == TernaryValue.Explored;
        }

        public void Insert(Atom a, bool overwrite = false)
        {
            if (a.Physical)
            {
                if (overwrite)
                {
                    table[a.Position] = a;
                }
                else
                {
                    if (CanMoveTo(a.Position))
                    {
                        buffer[a.Position] = table[a.Position];
                        table[a.Position] = a;
                    }
                    else
                    {
                        if (typeof(Floor).IsAssignableFrom(buffer[a.Position].GetType()))
                        {
                            buffer[a.Position] = a;
                        }
                    }
                }
            }
            else
            {
                untangibles[a.Position].Add(a);
            }
        }

        public void Insert(MoveableAtom a)
        {
            if (a.Physical)
            {
                if (a.Unblockable 
                    || CanMoveTo(a.Position)
                    || a.IsNotBlockedFromType(table[a.Position]))
                {
                    buffer[a.Position] = table[a.Position];
                    table[a.Position] = a;
                }
            }
            else
            {
                untangibles[a.Position].Add(a);
            }
        }

        public void Remove(Atom a)
        {
            if (a.Physical)
            {
                RemoveAt(a.Position);
            }
            else
            {
                untangibles[a.Position].Remove(a);
            }
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
            var floor = new Floor(coord);
            buffer[coord] = floor;
            floor.InsertInMap(this, coord);
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
            return table[coord].Walkable;
        }

        public void MoveAtomTo(Atom movedAtom, Coord prevPosition, Coord newPosition)
        {
            Atom steppedAtom = null;

            if (movedAtom.Physical)
            {
                table[prevPosition] = buffer[prevPosition];
                buffer[newPosition] = table[newPosition];
                table[newPosition] = movedAtom;
                steppedAtom = buffer[newPosition];
            }
            else
            {
                untangibles[prevPosition].Remove(movedAtom);
                untangibles[newPosition].Add(movedAtom);
                steppedAtom = table[newPosition]; ;
            }
            NotifyViewersOfMovement(movedAtom, prevPosition, newPosition);
           
            var steppedAtomType = steppedAtom.GetType();
            var movedAtomType = movedAtom.GetType();

            if (movedAtomType == typeof(SelectorCursor))
            {
                var exploredCell = steppedAtom.Map.IsFullyExplored(steppedAtom.Position);
                var msg = String.Format("{0}", exploredCell 
                                                ? steppedAtom.Description
                                                : new Floor().Description);

                movedAtom.NotifyListeners(msg);
            }
            else if (steppedAtomType.IsSubclassOf(typeof(Item)))
            {
                var item = ((Item)steppedAtom);
                var msg = String.Format("Stepped on {0}", item.ItemTypeName);

                var pgType = typeof(Pg);

                if (movedAtomType == pgType || movedAtomType.IsSubclassOf(pgType))
                {
                    steppedAtom.NotifyListeners(msg);
                    movedAtom.NotifyListeners(msg);
                }
            }
            else
            {
                table[prevPosition].NotifyListeners("");
            }
        }

        #endregion

        #region SERIALIZATION
        public Map(SerializationInfo info, StreamingContext context)
        {
            name = (string)info.GetValue(nameSerializableName, typeof(string));
            table = (BidimensionalArray<Atom>)info.GetValue(tableSerializableName, typeof(BidimensionalArray<Atom>));
            buffer = (BidimensionalArray<Atom>)info.GetValue(bufferSerializableName, typeof(BidimensionalArray<Atom>));
            dark = (BidimensionalArray<bool>)info.GetValue(darkSerializableName, typeof(BidimensionalArray<bool>));
            explored = (BidimensionalArray<TernaryValue>)info.GetValue(exploredSerializableName, typeof(BidimensionalArray<TernaryValue>));
            untangibles = (BidimensionalArray<AtomCollection>)info.GetValue(untangiblesSerializableName, typeof(BidimensionalArray<AtomCollection>));
            //views = (List<IMapViewer>)info.GetValue(viewsSerializableName, typeof(List<IMapViewer>));
            this.views = new List<IMapViewer>();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameSerializableName, name, typeof(string));
            info.AddValue(tableSerializableName, table, typeof(BidimensionalArray<Atom>));
            info.AddValue(bufferSerializableName, buffer, typeof(BidimensionalArray<Atom>));
            info.AddValue(exploredSerializableName, explored, typeof(BidimensionalArray<TernaryValue>));
            info.AddValue(darkSerializableName, dark, typeof(BidimensionalArray<bool>));
            info.AddValue(untangiblesSerializableName, untangibles, typeof(BidimensionalArray<AtomCollection>));
            //info.AddValue(viewsSerializableName, views, typeof(List<IMapViewer>));
        }

        public void Save(string filepath)
        {
            Stream outputStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(outputStream, this);
            outputStream.Close();
        }

        #endregion

        public void Explore(Coord pos, bool fullyExplored)
        {
            explored[pos] = fullyExplored ? TernaryValue.Explored : TernaryValue.Unknown;
        }

        public bool IsDark(Coord coord)
        {
            return dark[coord];
        }

#if DEBUG_PRINT_MAP
        public void SaveToTxt(string filepath)
        {
            var name = String.Format("[DEBUG]{0}", filepath);
            using (var w = new StreamWriter(name))
            {
                w.Write(this.ToString());
            }
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            var numByType = new Dictionary<Type, int>();

            str.AppendLine(String.Format("{0} [{1}x{2}]",
                            this.Name,
                            this.Width,
                            this.Height));

            var pos = new Coord();
            for (int r = 0; r < Height; r++)
            {
                pos.Y = r;
                for (int c = 0; c < Width; c++)
                {
                    pos.X = c;
                    var a = this[pos];
                    str.Append(a.Symbol);
                    var type = a.GetType();
                    if(numByType.ContainsKey(type))
                    {
                        numByType[type]++;
                    }
                    else
                    {
                        numByType[type] = 1;
                    }
                }
                str.AppendLine();
            }
            str.AppendLine();
            foreach (var key in numByType.Keys)
            {
                str.AppendLine(String.Format("{0}: {1}", key, numByType[key]));
            }
            return str.ToString();
        }

        public List<Atom> EveryAtom()
        {
            var lA = new List<Atom>();
            for (int c = 0; c < table.Cols; c++)
            {
                for (int r = 0; r < table.Rows; r++)
                {
                    lA.Add(table[r, c]);
                    lA.Add(buffer[r, c]);
                    lA.AddRange(untangibles[r, c]);
                }
            }

            return lA;
        }
#endif

    }
}