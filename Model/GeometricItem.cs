using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class GeometricItem
    {
        protected List<Coord> pts;

        public Coord this[int pos] { get { return pts[pos]; } }

        public IEnumerator GetEnumerator()
        {
            return new CircleEnum(pts.ToArray());
        }

        public Coord[] ToArray()
        {
            return pts.ToArray();
        }
    }
}
