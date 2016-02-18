//#define FIXED_OBJECT_TYPE

using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model.Core;
using System.Runtime.Serialization;
using GodsWill_ASCIIRPG.Model.Items;
using System.Reflection;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
    abstract public class Item : Atom, Descriptionable
    {
        public const int _UnlimitedUses = -1;

        int cost;
        int weight;
        int uses;

        public virtual int Cost { get { return cost; } }
        public virtual int Weight { get { return weight; } }
        public bool Expired { get { return uses == 0; } }
        public int Uses { get { return uses; } }

        public virtual bool IsSellable
        {
            get
            {
                return true;
            }
        }

        public virtual string ItemTypeName
        {
            get
            {
                //return this.GetType().Name.Clean();
                return this.Type.Name.Clean();
            }
        }

        public bool IsStackable
        {
            get
            {
                //return this.GetType().GetCustomAttributes(typeof(Stackable), false).Length > 0;
                return this.Attributes(typeof(Stackable), false).Count > 0;
            }
        }

        public abstract string FullDescription { get; }

        private Stackable stackable;
        private Stackable Stackable
        {
            get
            {
                if(stackable == null)
                {
                    //stackable = (Stackable)this.GetType().GetCustomAttributes(typeof(Stackable), false).FirstOrDefault();
                    stackable = (Stackable)this.Attributes(typeof(Stackable), false).FirstOrDefault();
                }

                return stackable;
            }
        }

        public int MaxPerStack
        {
            get
            {
                if(Stackable != null)
                {
                    return Stackable.MaxInStack;
                }
                return 0;
            }
        }

        public bool HasStackLimit
        {
            get
            {
                if (Stackable != null)
                {
                    return Stackable.HasStackLimit;
                }
                return true;
            }
        }

        public virtual bool SellableOnlyAsFullStack
        {
            get
            {
                if (Stackable != null)
                {
                    return Stackable.SellableOnlyAsFullStack;
                }
                return false;
            }
        }

        public Item(string name = "Generic Item",
                    string symbol = "i",
                    Color color = new Color(),
                    bool walkable = true,
                    bool blockVision = false,
                    string description = "Base element of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 0,
                    int uses = _UnlimitedUses)
            : base(name, symbol, color, walkable, blockVision, description, position)
        {
            this.cost = cost;
            this.weight = weight;
            this.uses = uses;

            IsPickable = true;
        }

        public Item(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        protected bool ConsumeUse()
        {
            if (uses > 0)
            {
                uses--;
            }

            return uses == 0;
        }

        public virtual void ActiveUse(Character user)
        {
            user.NotifyListeners("Mmm... It seems of no use");
        }

        public override bool Interaction(Atom interactor)
        {
            throw new NotImplementedException();
        }

        public static Item GenerateRandom(Pg.Level level, Coord position)
        {
            var generableItemsTypes = AppDomain.CurrentDomain.GetAssemblies()
                                                .SelectMany(s => s.GetTypes())
                                                .Where(p => p.GetCustomAttributes(typeof(RandomGenerable), false).Count() == 1)
                                                .ToArray();
#if FIXED_OBJECT_TYPE
            var ixGenerableItemsTypes = 0;
#else
            var ixGenerableItemsTypes = Dice.Throws(new Dice(generableItemsTypes.Length)) - 1;
#endif
            // Generator given family object
            var itemFamily = generableItemsTypes[ixGenerableItemsTypes];
            var type2 = typeof(ItemGenerator<>).MakeGenericType(new Type[] { itemFamily });
            //var type = typeof(ItemGenerator);
            var itemGeneratorClasses = AppDomain.CurrentDomain.GetAssemblies()
                                        .SelectMany(s => s.GetTypes())
                                        .Where(p => type2.IsAssignableFrom(p) && !p.IsAbstract)
                                        .ToArray();
#if FIXED_OBJECT_TYPE
            var ix = 0;
#else
            var ix = Dice.Throws(new Dice(itemGeneratorClasses.Length)) - 1;
#endif
            var luck = Dice.Throws(new Dice(20));
            var actualLevel = level;
            
            if(luck < 3)
            {
                actualLevel = actualLevel.Previous();
            }
            if(luck > 18)
            {
                actualLevel = actualLevel.Next();
            }

            var generator = (ItemGenerator)Activator.CreateInstance(itemGeneratorClasses[ix]);
            return generator.GenerateRandom(level, position);
        }
    }
}