using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public struct Coord
    {
        public int X;
        public int Y;

        public int SquaredDistanceFrom(Coord other)
        {
            var minusX = this.X - other.X;
            var minusY = this.Y - other.Y;
            return minusX * minusX + minusY * minusY;
        }
    }
}
