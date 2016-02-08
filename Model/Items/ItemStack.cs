using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [Serializable]
    public abstract class ItemStack : Item, ISerializable
    {
        public abstract int Count { get; }
        public abstract Type Containing { get; }
        public abstract bool IsEmpty { get; }
        public abstract Item Remove(Item item);
        public abstract void Add(Item item);

        public abstract Item this[int ix] { get; }
        public static ItemStack CreateByType(Type type)
        {
            var d1 = typeof(ItemStack<>);
            Type[] typeArgs = { type };
            var makeme = d1.MakeGenericType(typeArgs);
            return (ItemStack)Activator.CreateInstance(makeme);
        }

        public ItemStack()
            : base(name: "Item Stack")
        {

        }

        public abstract bool Contains(Item item);
    }

    [Serializable]
    public class ItemStack<TItem> : ItemStack, ISerializable
        where TItem : Item
    {
        private const string stackedSerializationName = "stacked";

        public override Item this[int ix]
        {
            get
            {
                return stacked[ix];
            }
        }

        public override int Count
        {
            get
            {
                return stacked.Count;
            }
        }

        public override Type Containing
        {
            get
            {
                return typeof(TItem);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return stacked.Count == 0;
            }
        }

        List<TItem> stacked;

        public ItemStack()
            : base()
        {
            stacked = new List<TItem>();
        }

        public ItemStack(SerializationInfo info, StreamingContext context)
        {
            stacked = (List<TItem>)info.GetValue(stackedSerializationName, typeof(List<TItem>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(stackedSerializationName, stacked, typeof(List<TItem>));
        }

        public override void ActiveUse(Character user)
        {
            if (Count > 0)
            {
                stacked[0].ActiveUse(user);
            }
        }

        public override Item Remove(Item item)
        {
            stacked.Remove((TItem)item);

            return item;
        }

        public override void Add(Item item)
        {
            stacked.Add((TItem)item);
        }

        public override bool Contains(Item item)
        {
            return stacked.Contains(item);
        }

        public override bool IsSellable
        {
            get
            {
                if (Count > 0)
                {
                    return stacked[0].IsSellable && (!stacked[0].SellableOnlyAsFullStack || Count == stacked[0].MaxPerStack);
                }
                return false;
            }
        }

        public override int Cost
        {
            get
            {
                var singleCost = Count > 0 ? stacked[0].Cost : 0;
                return SellableOnlyAsFullStack ? Count * singleCost : singleCost;
            }
        }

        public override bool SellableOnlyAsFullStack
        {
            get
            {
                if (Count > 0)
                {
                    return stacked[0].SellableOnlyAsFullStack;
                }

                return false;
            }
        }

        public override string Name
        {
            get
            {
                if (Count > 0)
                {
                    return String.Format("{0} x{1}{2}", 
                                         stacked[0].Name, 
                                         Count,
                                         stacked[0].HasStackLimit 
                                         ? String.Format("/{0}", stacked[0].MaxPerStack )
                                         : "");
                }

                return "UNEXPECTED";
            }
        }

        public override string FullDescription
        {
            get
            {
                if(Count > 0)
                {
                    return stacked[0].FullDescription;
                }

                return "UNEXPECTED";
            }
        }
    }
}
