using GodsWill_ASCIIRPG.Model;
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
                var O1 = new Coord(x + x0, y + y0); // Octant 1
                var O2 = new Coord(y + x0, x + y0); // Octant 2
                var O3 = new Coord(-x + x0, y + y0); // Octant 4
                var O4 = new Coord(-y + x0, x + y0); // Octant 3
                var O5 = new Coord(-x + x0, -y + y0); // Octant 5
                var O6 = new Coord(-y + x0, -x + y0); // Octant 6
                var O7 = new Coord(x + x0, -y + y0); // Octant 7
                var O8 = new Coord(y + x0, -x + y0); // Octant 8

                pts.AddRange(HorLine(O1, O3));
                pts.AddRange(HorLine(O2, O4));
                pts.AddRange(HorLine(O5, O7));
                pts.AddRange(HorLine(O6, O8));

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

        private List<Coord> HorLine(Coord a, Coord b)
        {
            int x0 = Math.Min(a.X, b.X);
            int x1 = Math.Max(a.X, b.X);
            int y = a.Y;

            List<Coord> pts = new List<Coord>();
            for (int x = x0; x < x1; x++)
            {
                pts.Add(new Coord(x, y));
            }

            return pts;
        }
    }
}
