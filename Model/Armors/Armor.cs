using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public class ArmorBuilder : ItemGenerator<Armor>
    {
        public override Armor GenerateTypedRandom(Pg.Level level, Coord position)
        {
            var armorType = typeof(Armor);

            // Requested power
            var allArmors = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(a => armorType.IsAssignableFrom(a)
                                    && !a.IsNestedPrivate
                                    && !a.IsAbstract)
                            .ToArray();
            var armors = allArmors.Where(w =>
            {
                var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                return attribute != null && ((Prerequisite)attribute).MinimumLevel == level;
            })
            .ToArray();

            // Lesser power
            if (armors.Length == 0)
            {
                armors =    allArmors
                            .Where(w =>
                            {
                                var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                                return attribute != null && ((Prerequisite)attribute).MinimumLevel < level;
                            })
                            .ToArray();
            }
            // No prerequisite
            if (armors.Length == 0)
            {
                armors = allArmors
                            .Where(w => w.GetCustomAttributes(typeof(Prerequisite), false).Count() == 0)
                            .ToArray();
            }

            if (armors.Length == 0)
            {
                throw new Exception("Unexpected No Armor");
            }

            var ix = Dice.Throws(new Dice(armors.Length)) - 1;

            var abstractGeneratorType = typeof(ItemGenerator<>).MakeGenericType(new Type[] { armors[ix] });
            var generatorType = AppDomain.CurrentDomain.GetAssemblies()
                                                       .SelectMany(s => s.GetTypes())
                                                       .Where(ag => abstractGeneratorType.IsAssignableFrom(ag)
                                                               && !ag.IsNestedPrivate
                                                               && !ag.IsAbstract).FirstOrDefault();
            if (generatorType == null)
            {
                throw new Exception("Unexpected No Armor Generator");
            }

            return (Armor)((ItemGenerator)Activator.CreateInstance(generatorType)).GenerateRandom(level, position);
        }
    }

    [Serializable]
    [RandomGenerable]
    public abstract class Armor : Item, ISerializable
    {
        public static readonly string DefaultSymbol = "[";

        private const string damageReductionSerializableName = "dmg";
        private const string armorTypeSerializableName = "type";

        public enum _ArmorType
        {
            No_Armor = 1,
            Light_Armor = 2,
            Medium_Armor = 4,
            Heavy_Armor = 8
        }

        private class _Skin : Armor
        {
            public _Skin()
                : base("Skin",
                      Armor.DefaultSymbol,
                      Color.White,
                      new Damage(),
                      _ArmorType.No_Armor)
            {
                IsSkin = true;
            }
        }

        private static readonly _Skin skin = new _Skin();
        public static Armor Skin
        {
            get
            {
                return skin;
            }
        }

        private Damage damageReduction;
        public Damage DamageReduction { get { return damageReduction; } }

        public bool IsSkin { get; protected set; }
        public _ArmorType ArmorType { get; private set; }
        public int Fatigue { get { return (int)this.ArmorType; }}

        public Armor(string name,
                    string symbol,
                    Color color,
                    Damage damageReduction,
                    _ArmorType armorType,
                    string description = "Base armor of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 1,
                    int uses = Item._UnlimitedUses)
            : base(name, symbol, color, true, false, description, position, cost, weight, uses)
        {
            this.damageReduction = damageReduction;
            this.ArmorType = armorType;
        }

        public Armor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            damageReduction = (Damage)info.GetValue(damageReductionSerializableName, typeof(Damage));
            ArmorType = (_ArmorType)info.GetValue(armorTypeSerializableName, typeof(_ArmorType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(damageReductionSerializableName, damageReduction, typeof(Damage));
            info.AddValue(armorTypeSerializableName, ArmorType, typeof(_ArmorType));
        }

        public override string FullDescription
        {
            get
            {
                var str = new StringBuilder();
                str.AppendLine(String.Format("{0} [{1}]", Name, ItemTypeName));
                str.AppendLine(String.Format("Cost: {0} Weight{1}",
                                             Cost,
                                             Weight));
                str.AppendLine();
                str.AppendLine(damageReduction.ToString());
                str.AppendLine();
                str.AppendLine(Description);
                return str.ToString();
            }
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine(String.Format("{0}[{1}]",
                                         Name,
                                         ItemTypeName));
            str.AppendLine(ArmorType.ToString().Clean());
            str.AppendLine(DamageReduction.ToString());

            return str.ToString();
        }
    }
}