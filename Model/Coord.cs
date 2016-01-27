using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public struct Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coord(Coord c)
        {
            X = c.X;
            Y = c.Y;
        }

        public static Coord Random(int width, int height)
        {
            int x = Dice.Throws(width + 1) - 1;
            int y = Dice.Throws(height + 1) - 1;

            return new Coord() { X = x, Y = y };
        }

        public int SquaredDistanceFrom(Coord other)
        {
            var minusX = this.X - other.X;
            var minusY = this.Y - other.Y;
            return minusX * minusX + minusY * minusY;
        }

        public static Coord operator +(Coord pt1, Coord pt2)
        {
            return new Coord()
            {
                X = pt1.X + pt2.X,
                Y = pt1.Y + pt2.Y,
            };
        }

        public static Coord operator -(Coord pt1, Coord pt2)
        {
            return new Coord()
            {
                X = pt1.X - pt2.X,
                Y = pt1.Y - pt2.Y,
            };
        }

        public static bool operator ==(Coord pt1, Coord pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static bool operator !=(Coord pt1, Coord pt2)
        {
            return pt1.X != pt2.X || pt1.Y != pt2.Y;
        }

        public override string ToString()
        {
            return String.Format("[{0};{1}]", X, Y);
        }
    }
}
