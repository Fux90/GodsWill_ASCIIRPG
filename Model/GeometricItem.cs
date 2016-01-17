using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class GeometricItem : IEnumerable
    {
        protected List<Coord> pts;

        public Coord this[int pos] { get { return pts[pos]; } }

        public IEnumerator GetEnumerator()
        {
            return new GeometricItemEnum(pts.ToArray());
        }

        public Coord[] ToArray()
        {
            return pts.ToArray();
        }
    }

    public class GeometricItemEnum : IEnumerator
    {
        Coord[] pts;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public GeometricItemEnum(Coord[] _pts)
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
