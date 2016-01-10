using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG
{
    public abstract class Shield : Item
	{
        public static readonly string DefaultSymbol = "(";

        private class _NoShield : Shield
        {
            public _NoShield()
                : base("No Shield",
                      Shield.DefaultSymbol,
                      Color.White,
                      0,
                      0,
                      false)
            {
                IsSkin = true;
            }
        }

        private static readonly _NoShield noShield = new _NoShield();
        public static Shield NoShield
        {
            get
            {
                return noShield;
            }
        }

        private int bonusCA;
        public int BonusCA { get { return bonusCA; } }
        private int bonusSpecialCA;
        public int BonusSpecialCA { get { return bonusSpecialCA; } }

        public bool IsSkin { get; protected set; }

        public Shield(string name,
                    string symbol,
                    Color color,
                    int bonusCA,
                    int bonusSpecialCA,
                    bool walkable = true,
                    string description = "Base shield of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 1,
                    int uses = Item._UnlimitedUses)
            : base(name, symbol, color, true, description, position, cost, weight, uses)
        {
            this.bonusCA = bonusCA;
            this.bonusSpecialCA = bonusSpecialCA;
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
                str.AppendLine(String.Format("CA: {0}", BonusCA));
                str.AppendLine(String.Format("Special: {0}", BonusSpecialCA));
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
            str.AppendLine(String.Format("Bonus CA: {0}", BonusCA));
            str.AppendLine(String.Format("Bonus Special: {0}", BonusCA));
            return str.ToString();
        }
    }
}