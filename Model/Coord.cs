﻿using GodsWill_ASCIIRPG.Model.Core;
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

        /// <summary>
        /// Returns a random position inside a map, given its dimensions
        /// </summary>
        /// <param name="width">Width of Map</param>
        /// <param name="height">Height of Map</param>
        /// <returns>Coordinate (x,y) | x in [0; width-1], y in [0; height-1]</returns>
        public static Coord Random(int width, int height)
        {
            int x = Dice.Throws(width) - 1;
            int y = Dice.Throws(height) - 1;

            return new Coord() { X = x, Y = y };
        }

        public int SquaredDistanceFrom(Coord other)
        {
            var minusX = this.X - other.X;
            var minusY = this.Y - other.Y;
            return minusX * minusX + minusY * minusY;
        }

        public int ManhattanDistance(Coord other)
        {
            return Math.Abs(this.Y - other.Y) + Math.Abs(this.X - other.X);
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
