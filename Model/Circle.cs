using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class Circle : GeometricItem, IEnumerable
    {
        public Coord Center { get; private set; }
        public int Radius{ get; private set; }

        public Circle(Coord center, int radius)
        {
            Center = center;
            Radius = radius;
        }
    }

    public class SimpleCircle : Circle 
    {
        public SimpleCircle(Coord center, int radius)
            : base(center, radius)
        {
            pts = new List<Coord>();

            int x0 = center.X;
            int y0 = center.Y;
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;   // Decision criterion divided by 2 evaluated at x=r, y=0

            while (y <= x)
            {
                pts.Add(new Coord(x + x0, y + y0)); // Octant 1
                pts.Add(new Coord(y + x0, x + y0)); // Octant 2
                pts.Add(new Coord(-x + x0, y + y0)); // Octant 4
                pts.Add(new Coord(-y + x0, x + y0)); // Octant 3
                pts.Add(new Coord(-x + x0, -y + y0)); // Octant 5
                pts.Add(new Coord(-y + x0, -x + y0)); // Octant 6
                pts.Add(new Coord(x + x0, -y + y0)); // Octant 7
                pts.Add(new Coord(y + x0, -x + y0)); // Octant 8
                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }        
    }

    public class FilledCircle : Circle
    {
        public FilledCircle(Coord center, int radius)
            : base(center, radius)
        {
            pts = new List<Coord>();

            int x0 = center.X;
            int y0 = center.Y;
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;   // Decision criterion divided by 2 evaluated at x=r, y=0

            while (y <= x)
            {
                pts.Add(new Coord(x + x0, y + y0)); // Octant 1
                pts.Add(new Coord(y + x0, x + y0)); // Octant 2
                pts.Add(new Coord(-x + x0, y + y0)); // Octant 4
                pts.Add(new Coord(-y + x0, x + y0)); // Octant 3
                pts.Add(new Coord(-x + x0, -y + y0)); // Octant 5
                pts.Add(new Coord(-y + x0, -x + y0)); // Octant 6
                pts.Add(new Coord(x + x0, -y + y0)); // Octant 7
                pts.Add(new Coord(y + x0, -x + y0)); // Octant 8
                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
    }
    }

    public class CircleEnum : IEnumerator
    {
        Coord[] pts;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public CircleEnum(Coord[] _pts)
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
