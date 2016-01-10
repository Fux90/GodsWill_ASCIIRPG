using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class Backpack : IEnumerable
    {
        List<Item> items;

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
        }

        public void Add(Item item)
        {
            this.items.Add(item);
        }

        public Item Remove(Item item)
        {
            return RemoveAt(this.items.IndexOf(item));
        }

        public Item RemoveAt(int index)
        {
            var removedObject = items[index];
            items.RemoveAt(index);
            return removedObject;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public BackpackEnum GetEnumerator()
        {
            return new BackpackEnum(items.ToArray());
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
