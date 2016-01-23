using GodsWill_ASCIIRPG.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class Spellbook : IEnumerable
    {
        List<Spell> spells;
        List<ISpellbookViewer> spellbookViewers;

        public int Count
        {
            get { return spells.Count; }
        }

        public Spell this[int index]
        {
            get
            {
                return spells[index];
            }
        }

        public Spellbook(List<Spell> items = null)
        {
            this.spells = items != null ? items : new List<Spell>();
            spellbookViewers = new List<ISpellbookViewer>();
        }

        public void Add(Spell spell)
        {
            this.spells.Add(spell);
            NotifyAdd();
        }

        public Spell Remove(Spell spell)
        {
            NotifyAdd();
            return RemoveAt(this.spells.IndexOf(spell));
        }

        public Spell RemoveAt(int index)
        {
            var removedSpell = spells[index];
            spells.RemoveAt(index);
            return removedSpell;
        }

        public void RegisterViewer(ISpellbookViewer viewer)
        {
            spellbookViewers.Add(viewer);
            NotifyAdd();
        }

        public void NotifyAdd()
        {
            spellbookViewers.ForEach(viewer => viewer.NotifyAdd(this.ToArray()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public SpellbookEnum GetEnumerator()
        {
            return new SpellbookEnum(spells.ToArray());
        }

        public Spell[] ToArray()
        {
            if (this.Count > 0)
            {
                var res = new Spell[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    res[i] = this[i];
                }
                return res;
            }

            return new Spell[] { };
        }

        public List<Spell> ToList()
        {
            if (this.Count > 0)
            {
                var res = new List<Spell>(this.Count);
                for (int i = 0; i < this.Count; i++)
                {
                    res.Add(this[i]);
                }
                return res;
            }

            return new List<Spell>();
        }
    }

    public class SpellbookEnum : IEnumerator
    {
        Spell[] spells;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public SpellbookEnum(Spell[] list)
        {
            spells = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < spells.Length);
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

        public Spell Current
        {
            get
            {
                try
                {
                    return spells[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
