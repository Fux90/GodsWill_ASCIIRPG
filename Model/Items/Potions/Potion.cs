using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items.Potions
{
    [Serializable]
    [Stackable(50)]
    public abstract class Potion : IdentifiableItem
    {
        public override bool IsStackable
        {
            get
            {
                return true;
            }
        }

        private Stackable stackable;
        protected override Stackable Stackable
        {
            get
            {
                if (stackable == null)
                {
                    stackable = (Stackable)typeof(Potion).GetCustomAttributes(typeof(Stackable), false).FirstOrDefault();
                }

                return stackable;
            }
        }

        public override int MaxPerStack
        {
            get
            {
                if (Stackable != null)
                {
                    return Stackable.MaxInStack;
                }
                return 0;
            }
        }

        public override bool HasStackLimit
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

        public override bool SellableOnlyAsFullStack
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

        public Potion(  string name = "Potion", 
                        Coord position = new Coord(),
                        string description = "Base potion",
                        int cost = 1,
                        int weight = 1)
            : base(name, 
                  "!", 
                  description: description,
                  position: position,
                  cost: cost,
                  weight: weight,
                  uses: 1)
        {

        }

        public Potion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        public override string Name
        {
            get
            {
                return String.Format("Potion of {0}", base.Name.Replace("Potion", ""));
                //return base.Name.Replace("Potion", "");
            }
        }
    }
}
