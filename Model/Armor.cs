using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public abstract class Armor : Item
	{
        public static readonly string DefaultSymbol = "[";

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
                      _ArmorType.No_Armor,
                      false)
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
                    bool walkable = true,
                    string description = "Base armor of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 1,
                    int uses = Item._UnlimitedUses)
            : base(name, symbol, color, true, description, position, cost, weight, uses)
        {
            this.damageReduction = damageReduction;
            this.ArmorType = armorType;
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
                                         ItemTypeName.Clean()));
            str.AppendLine(ArmorType.ToString().Clean());
            str.AppendLine(DamageReduction.ToString());

            return str.ToString();
        }
    }
}