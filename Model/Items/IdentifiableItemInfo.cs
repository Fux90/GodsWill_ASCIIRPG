using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public static class IdentifiableItemInfo
    {
        public class ItemInfo
        {
            public Color Color {get;set;}
            public string RandomName { get; set; }

            public ItemInfo()
            {

            }

            public ItemInfo(BinaryReader bR)
            {
                Color = Color.FromArgb(bR.ReadInt32());
                RandomName = bR.ReadString();
            }

            public void Save(BinaryWriter bW)
            {
                bW.Write(Color.ToArgb());
                bW.Write(RandomName);
            }
        }

        static Dictionary<Type, ItemInfo> randomInfos;
        static Dictionary<Type, bool> identified;

        public static ItemInfo InfosOfType(Type type)
        {
            if(randomInfos.ContainsKey(type))
            {
                return randomInfos[type];
            }

            return null;
        }

        public static bool IsIdentified(this Type type)
        {
            if(identified.ContainsKey(type))
            {
                return identified[type];
            }

            return true;
        }

        static IdentifiableItemInfo()
        {
            randomInfos = new Dictionary<Type, ItemInfo>();
            identified = new Dictionary<Type, bool>();

            var identifiableType = typeof(IdentifiableItem);
            var identifiables = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetTypes())
                                .Where(i => identifiableType.IsAssignableFrom(i)
                                    && !i.IsNestedPrivate
                                    && !i.IsAbstract)
                                .ToArray();
            var groupedIdentifiables = from identifiable in identifiables
                                       let a = (Identifiable)identifiable.GetCustomAttributes(typeof(Identifiable), false).FirstOrDefault()
                                       //group new { Type = identifiable, MacroType = a.MacroType } by a.MacroType;
                                       group new { Type = identifiable } by a.MacroType;

            var colors = new Color[]
            {
                Color.Blue,
                Color.Red,
                Color.Yellow,
                Color.YellowGreen,
                Color.Purple,
                Color.Orange,
                Color.Olive,
            };

            foreach (var group in groupedIdentifiables)
            {
                var itemsCount = group.Count();
                
                for (int i = 0; i < itemsCount; i++)
                {
                    var curType = (Type)group.ElementAt(i).Type;
                    identified[curType] = false;
                    randomInfos[curType] = new ItemInfo()
                    {
                        Color = colors[Dice.Throws(colors.Length)],
                        RandomName = generateRandomName(),
                    };
                }
            }
        }

        private static string generateRandomName()
        {
            var strB = new StringBuilder();
            var len = Dice.Throws(3, 4);
            for (int i = 0; i < len; i++)
            {
                strB.AppendFormat("{0}", Convert.ToChar(Dice.Throws(26) + 64));
            }
            return strB.ToString();
        }

        public static void Identify(Type type)
        {
            identified[type] = true;
        }

        public static void Save(BinaryWriter bW)
        {
            bW.Write(randomInfos.Count);
            foreach (var key in randomInfos.Keys)
            {
                bW.Write(key.ToString());
                randomInfos[key].Save(bW);
                bW.Write(identified[key]);
            }
        }

        public static void Load(BinaryReader bR)
        {
            var count = bR.ReadInt32();

            randomInfos = new Dictionary<Type, ItemInfo>();
            identified = new Dictionary<Type, bool>();

            for (int i = 0; i < count; i++)
            {
                var type = Type.GetType(bR.ReadString());
                randomInfos[type] = new ItemInfo(bR);
                identified[type] = bR.ReadBoolean();
            }
        }
    }


}
