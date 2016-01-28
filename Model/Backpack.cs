using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public class Backpack : IEnumerable
    {
        List<Item> items;
        [NonSerialized]
        List<IBackpackViewer> backPackViewers;
        List<IBackpackViewer> BackPackViewers
        {
            get
            {
                if(backPackViewers == null)
                {
                    backPackViewers = new List<IBackpackViewer>();
                }
                return backPackViewers;
            }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public Item this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public Backpack(List<Item> items = null)
        {
            this.items = items != null ? items : new List<Item>();
            //backPackViewers = new List<IBackpackViewer>();
        }

        public void Add(Item item)
        {
            this.items.Add(item);
            NotifyAddRemoval();
        }

        public Item Remove(Item item)
        {
            var itemR = RemoveAt(this.items.IndexOf(item));
            NotifyAddRemoval();
            return itemR;
        }

        public Item RemoveAt(int index)
        {
            var removedObject = items[index];
            items.RemoveAt(index);
            return removedObject;
        }

        public void RegisterViewer(IBackpackViewer viewer)
        {
            BackPackViewers.Add(viewer);
            NotifyAddRemoval();
        }

        public void NotifyAddRemoval()
        {
            BackPackViewers.ForEach( viewer => viewer.NotifyAddRemoval(this.ToArray()) );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public BackpackEnum GetEnumerator()
        {
            return new BackpackEnum(items.ToArray());
        }

        public Item[] ToArray()
        {
            if (this.Count > 0)
            {
                var res = new Item[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    res[i] = this[i];
                }
                return res;
            }

            return new Item[] { };
        }

        public List<Item> ToList()
        {
            if (this.Count > 0)
            {
                var res = new List<Item>(this.Count);
                for (int i = 0; i < this.Count; i++)
                {
                    res.Add(this[i]);
                }
                return res;
            }

            return new List<Item>();
        }
    }

    public class BackpackEnum : IEnumerator
    {
        Item[] items;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public BackpackEnum(Item[] list)
        {
            items = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < items.Length);
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

        public Item Current
        {
            get
            {
                try
                {
                    return items[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
