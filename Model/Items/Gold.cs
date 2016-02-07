using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    [Serializable]
    public class Gold : Atom
    {
        #region SERIALIZABLE_CONST_NAMES
        const string amountSerializableName = "gold";
        #endregion

        public int Amount { get; private set; }

        public Gold(int amount, Coord position = default(Coord)) 
                    : base("Gold", "$", Color.Gold, false, false, "A pocket of gold", position)
        {
            this.Amount = amount;
        }

        public Gold(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Amount = (int)info.GetValue(amountSerializableName, typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(amountSerializableName, Amount, typeof(int));
        }

        public override bool Interaction(Atom interactor)
        {
            if (typeof(IGoldDealer).IsAssignableFrom(interactor.GetType()))
            {
                ((IGoldDealer)interactor).PickUpGold(this);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
