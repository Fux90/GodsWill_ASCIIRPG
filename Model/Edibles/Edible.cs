using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Edibles
{
    [Serializable]
    public abstract class Edible : Item, ISerializable
    {
        const string edibleValueSerializeableName = "edibleV";

        public int EdibleValue { get; private set; }

        public Edible(  int edibleValue,
                        string name,
                        string symbol = "%",
                        Color? color = null,
                        Coord position = new Coord(),
                        int cost = 1,
                        int weight = 1,
                        int uses = 1)
            : base(name,
                  symbol,
                  color == null 
                  ? Color.Brown
                  : (Color)color,
                  true,
                  false,
                  "Generic food",
                  position,
                  cost,
                  weight,
                  uses)
        {
            this.EdibleValue = edibleValue;
        }

        public Edible(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EdibleValue = (int)info.GetValue(edibleValueSerializeableName, typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(edibleValueSerializeableName, EdibleValue, typeof(int));
        }

        public override void ActiveUse(Character user)
        {
            user.Eat(this);
        }

        public override string FullDescription
        {
            get
            {
                var str = new StringBuilder();

                str.AppendLine(this.Name);
                str.AppendLine(String.Format("Edible Value: {0}", EdibleValue));

                return str.ToString();
            }
        }
    }
}
