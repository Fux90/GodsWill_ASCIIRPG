using GodsWill_ASCIIRPG.Model.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [Serializable]
    public class Backpack : IEnumerable, IViewable<IBackpackViewer>
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
            if (item.IsStackable)
            {
                var stacks = items.Where(i => i.GetType().IsSubclassOf(typeof(ItemStack)));

                ItemStack currStack;
                int ix = 0;
                do
                {
                    currStack = (ItemStack)stacks.ElementAtOrDefault(ix);
                    ix++;
                }
                while (currStack != null && item.HasStackLimit && currStack.Count == item.MaxPerStack);

                if (currStack == null)
                {
                    // No stacks of given type already exist
                    // or all are full
                    var stack = ItemStack.CreateByType(item.GetType());
                    this.items.Add(stack);
                    currStack = stack;
                }

                currStack.Add(item);
            }
            else
            {
                this.items.Add(item);
            }

            NotifyAddRemoval();
        }

        public Item Remove(Item item)
        {
            var ix = this.items.IndexOf(item);
            // If not found in backpack, it could be in a stack
            if (ix == -1)
            {
                var firstStack = (ItemStack)items.Where(i => i.GetType().IsSubclassOf(typeof(ItemStack)))
                                                 .Where(s => ((ItemStack)s).Containing == item.GetType())
                                                 .FirstOrDefault(s => ((ItemStack)s).Contains(item));
                if(firstStack == null)
                {
                    throw new Exception("Unexpected not contained Item");
                }

                var itemR = firstStack.Remove(item);
                if(firstStack.IsEmpty)
                {
                    this.items.Remove(firstStack);
                }

                NotifyAddRemoval();
                return itemR;
            }
            else
            {
                var itemR = RemoveAt(ix);
                NotifyAddRemoval();
                return itemR;
            }
        }

        public Item RemoveAt(int index)
        {
            var removedObject = items[index];
            items.RemoveAt(index);
            return removedObject;
        }

        public void RegisterView(IBackpackViewer viewer)
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
