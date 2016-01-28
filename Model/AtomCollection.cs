using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public class AtomCollection : Atom, ICollection<Atom>
    {
        const string atomsSerializeableName = "atoms";

        List<Atom> atoms;

        public int Count
        {
            get
            {
                return atoms.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public AtomCollection()
            : base("Collection", "C", Color.White, true, false)
        {
            atoms = new List<Atom>();
        }

        public AtomCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            atoms = (List<Atom>)info.GetValue(atomsSerializeableName, typeof(List<Atom>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(atomsSerializeableName, atoms, typeof(List<Atom>));
        }

        public Atom this[int index]
        {
            get
            {
                return atoms[index];
            }
        }

        public void Add(Atom atom)
        {
            atoms.Remove(atom); //Prevent duplication
            atoms.Add(atom);
        }

        public void AddRange(List<Atom> atomList)
        {
            atoms.AddRange(atomList);
        }

        public void Clear()
        {
            atoms.Clear();
        }
        
        public bool Contains(Atom atom)
        {
            return atoms.Contains(atom);
        }

        public void CopyTo(Atom[] array, int arrayIndex)
        {
            atoms.CopyTo(array, arrayIndex);
        }

        public bool Remove(Atom atom)
        {
            return atoms.Remove(atom);
        }

        public override bool Interaction(Atom interactor)
        {
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public AtomCollectionEnum GetEnumerator()
        {
            return new AtomCollectionEnum(atoms.ToArray());
        }

        IEnumerator<Atom> IEnumerable<Atom>.GetEnumerator()
        {
            return new AtomCollectionEnum(atoms.ToArray());
        }

        public void ForEach(Action<Atom> action)
        {
            atoms.ForEach(action);
        }
    }

    public class AtomCollectionEnum : IEnumerator<Atom>
    {
        Atom[] atoms;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public AtomCollectionEnum(Atom[] list)
        {
            atoms = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < atoms.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            //
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Atom Current
        {
            get
            {
                try
                {
                    return atoms[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
