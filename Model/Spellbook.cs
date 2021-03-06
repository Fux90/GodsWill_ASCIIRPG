﻿using GodsWill_ASCIIRPG.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public class Spellbook : IEnumerable, IViewable<ISpellbookViewer>
    {
        List<SpellBuilder> spells;
        [NonSerialized]
        List<ISpellbookViewer> spellbookViewers;
        List<ISpellbookViewer> SpellbookViewers
        {
            get
            {
                if(spellbookViewers == null)
                {
                    spellbookViewers = new List<ISpellbookViewer>();
                }
                return spellbookViewers;
            }
        }
        [NonSerialized]
        IAtomListener spellSecondaryView;

        public int Count
        {
            get { return spells.Count; }
        }

        public SpellBuilder this[int index]
        {
            get
            {
                return spells[index];
            }
        }

        public Spellbook(List<SpellBuilder> items = null, IAtomListener spellSecondaryView = null)
        {
            this.spells = items != null ? items : new List<SpellBuilder>();
            //spellbookViewers = new List<ISpellbookViewer>();
            this.spellSecondaryView = spellSecondaryView;
        }

        public bool Contains(SpellBuilder spell)
        {
            return this.spells.Contains(spell);
        }

        public IEnumerable<SpellBuilder> Where(Func<SpellBuilder, bool> predicate)
        {
            var lst = new List<SpellBuilder>();

            foreach(SpellBuilder sB in this.spells)
            {
                if (predicate(sB))
                {
                    lst.Add(sB);
                }
            }            

            return lst;
        }

        public bool Add(SpellBuilder spell)
        {
            if (this.spells.Contains(spell))
            {
                return false;
            }
            else
            { 
                this.spells.Add(spell);
                spell.RegisterView(spellSecondaryView);
                NotifyAdd();
                return true;
            }
        }

        public SpellBuilder Remove(SpellBuilder spell)
        {
            NotifyAdd();
            return RemoveAt(this.spells.IndexOf(spell));
        }

        public SpellBuilder RemoveAt(int index)
        {
            var removedSpell = spells[index];
            spells.RemoveAt(index);
            return removedSpell;
        }

        public void RegisterView(ISpellbookViewer viewer)
        {
            SpellbookViewers.Add(viewer);
            NotifyAdd();
        }

        public void RegisterSecondaryView(IAtomListener spellSecondaryView)
        {
            this.spellSecondaryView = spellSecondaryView;
        }

        public void NotifyAdd()
        {
            SpellbookViewers.ForEach(viewer => viewer.NotifyAdd(this.ToArray()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public SpellbookEnum GetEnumerator()
        {
            return new SpellbookEnum(spells.ToArray());
        }

        public SpellBuilder[] ToArray()
        {
            if (this.Count > 0)
            {
                var res = new SpellBuilder[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    res[i] = this[i];
                }
                return res;
            }

            return new SpellBuilder[] { };
        }

        public List<SpellBuilder> ToList()
        {
            if (this.Count > 0)
            {
                var res = new List<SpellBuilder>(this.Count);
                for (int i = 0; i < this.Count; i++)
                {
                    res.Add(this[i]);
                }
                return res;
            }

            return new List<SpellBuilder>();
        }
    }

    public class SpellbookEnum : IEnumerator
    {
        SpellBuilder[] spells;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public SpellbookEnum(SpellBuilder[] list)
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

        public SpellBuilder Current
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
