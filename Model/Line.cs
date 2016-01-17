using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class Line : GeometricItem, IEnumerable
    {
        public Line(Coord ptA, Coord ptB)
        {
            pts = new List<Coord>();

            int x0 = ptA.X;
            int y0 = ptA.Y;
            int x1 = ptB.X;
            int y1 = ptB.Y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Utilities.Swap<int>(ref x0, ref y0); Utilities.Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Utilities.Swap<int>(ref x0, ref x1); Utilities.Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                pts.Add(steep ? new Coord(y, x) : new Coord(x, y));
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }

            if (pts.Count > 0 && pts[0] != ptA)
            {
                pts.Reverse();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new LineEnum(pts.ToArray());
        }
    }

    public class LineEnum : IEnumerator
    {
        Coord[] pts;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public LineEnum(Coord[] _pts)
        {
            pts = _pts;
        }

        public bool MoveNext()
        {
            position++;
            return (position < pts.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Coord Current
        {
            get
            {
                try
                {
                    return pts[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
