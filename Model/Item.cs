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
    abstract public class Item : Atom, Descriptionable, IIdentifiable
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

        

        public abstract string FullDescription { get; }

        private Stackable stackable;
        protected virtual Stackable Stackable
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

        public virtual bool IsStackable
        {
            get
            {
                return this.Attributes(typeof(Stackable), false).Count > 0;
            }
        }

        public virtual int MaxPerStack
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

        public virtual bool HasStackLimit
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

        public virtual bool IsIdentified
        {
            get
            {
                return true;
            }
        }

        public Item(string name = "Generic Item",
                    string symbol = "i",
                    Color color = new Color(),
                    bool walkable = true,
                    bool blockVision = false,
                    string description = "Base item of the game",
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

        public virtual void Identify(Atom identifier, int throwToIdentify)
        {
            // By default an object is already identified
        }
    }

    [Serializable]
    abstract public class IdentifiableItem : Item
    {
        int cd;
        private int CD
        {
            get
            {
                if(cd == -1)
                {
                    cd = ((Identifiable)this.Attributes(typeof(Identifiable))[0]).CD;
                }
                return cd;
            }
        }
        private IdentifiableItemInfo.ItemInfo randomInfo;
        protected IdentifiableItemInfo.ItemInfo RandomInfo
        {
            get
            {
                if(randomInfo == null)
                {
                    randomInfo = IdentifiableItemInfo.InfosOfType(this.Type);
                }
                return randomInfo;
            }
        }

        public override bool IsIdentified
        {
            get
            {
                return IdentifiableItemInfo.IsIdentified(this.Type);
            }
        }

        public override Color Color
        {
            get
            {
                return RandomInfo.Color;
            }
        }

        public override string Name
        {
            get
            {
                return IsIdentified ? base.Name : RandomInfo.RandomName;
            }
        }

        public IdentifiableItem(string name = "Generic Identifiable Item",
                                string symbol = "i",
                                Color color = new Color(),
                                bool walkable = true,
                                bool blockVision = false,
                                string description = "Base identifiable item of the game",
                                Coord position = new Coord(),
                                int cost = 0,
                                int weight = 0,
                                int uses = _UnlimitedUses)
            : base(name, symbol, color, walkable, blockVision, description, position)
        {
            
        }

        public IdentifiableItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override void ActiveUse(Character user)
        {
            if (!IdentifiableItemInfo.IsIdentified(this.GetType()))
            {
                Identify(user, int.MaxValue);
            }
        }

        public override void Identify(Atom identifier, int throwToIdentify)
        {
            if(throwToIdentify >= CD)
            {
                IdentifiableItemInfo.Identify(this.Type);
                identifier.NotifyListeners(String.Format("Identified {0}", this.Name));
            }
            else
            {
                identifier.NotifyListeners(String.Format("{0} not identified...", this.ItemTypeName));
            }
        }

        public override string ItemTypeName
        {
            get
            {
                return ((Identifiable)this.Attributes(typeof(Identifiable))[0]).MacroType.Name;
            }
        }
    }
}