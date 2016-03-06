using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System.Runtime.Serialization;
using GodsWill_ASCIIRPG.Model.Items;

namespace GodsWill_ASCIIRPG
{
    public class ShieldBuilder : ItemGenerator<Shield>
    {
        public override Shield GenerateTypedRandom(Pg.Level level, Coord position, RarenessValue rareness)
        {
            var shieldType = typeof(Shield);

            // Requested power
            var allShields = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(s => shieldType.IsAssignableFrom(s)
                                    && !s.IsNestedPrivate
                                    && !s.IsAbstract)
                            .ToArray();
            var shields = allShields.Where(w =>
            {
                var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                var sRareness = w.GetCustomAttributes(typeof(RarenessValue), false).FirstOrDefault();
                
                return attribute != null && ((Prerequisite)attribute).MinimumLevel == level
                        && sRareness != null && ((Rareness)sRareness).Value == rareness
                        && w.IsGenerable();
            })
            .ToArray();

            // Lesser power
            if (shields.Length == 0)
            {
                shields = allShields
                            .Where(s =>
                            {
                                var attribute = s.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                                var sRareness = s.GetCustomAttributes(typeof(RarenessValue), false).FirstOrDefault();
                                        
                                return attribute != null && ((Prerequisite)attribute).MinimumLevel < level
                                        && sRareness != null && ((Rareness)sRareness).Value <= rareness
                                        && s.IsGenerable(); ;
                            })
                            .ToArray();
            }
            // No prerequisite
            if (shields.Length == 0)
            {
                shields = allShields
                            .Where(s => {
                                var wRareness = s.GetCustomAttributes(typeof(RarenessValue), false).FirstOrDefault();
                                        
                                return s.GetCustomAttributes(typeof(Prerequisite), false).Count() == 0
                                        && (wRareness == null || ((Rareness)wRareness).Value <= rareness)
                                        && s.IsGenerable();
                            })
                            .ToArray();
            }

            if (shields.Length == 0)
            {
                throw new Exception("Unexpected No Shield");
            }

            var ix = Dice.Throws(new Dice(shields.Length)) - 1;

            var abstractGeneratorType = typeof(ItemGenerator<>).MakeGenericType(new Type[] { shields[ix] });
            var generatorType = AppDomain.CurrentDomain.GetAssemblies()
                                                       .SelectMany(s => s.GetTypes())
                                                       .Where(sg => abstractGeneratorType.IsAssignableFrom(sg)
                                                               && !sg.IsNestedPrivate
                                                               && !sg.IsAbstract).FirstOrDefault();
            if (generatorType == null)
            {
                throw new Exception("Unexpected No Shield Generator");
            }

            var shieldSpecificRareness = Item.Rareness();
            return (Shield)((ItemGenerator)Activator.CreateInstance(generatorType)).GenerateRandom(level, position, shieldSpecificRareness);
        }
    }

    [Serializable]
    [RandomGenerable]
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
                      0)
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
                    string description = "Base shield of the game",
                    Coord position = new Coord(),
                    int cost = 0,
                    int weight = 1,
                    int uses = Item._UnlimitedUses)
            : base(name, symbol, color, true, false, description, position, cost, weight, uses)
        {
            this.bonusCA = bonusCA;
            this.bonusSpecialCA = bonusSpecialCA;
        }

        public Shield(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

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
                                         ItemTypeName));
            str.AppendLine(String.Format("Bonus CA: {0}", BonusCA));
            str.AppendLine(String.Format("Bonus Special: {0}", BonusCA));
            return str.ToString();
        }
    }
}