using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public static class ArtifactItemInfo
    {
        static Dictionary<Type, int> artifactGenerated;

        public static bool IsGenerable(this Type type)
        {
            if(artifactGenerated.ContainsKey(type))
            {
                return artifactGenerated[type] > 0;
            }

            return true;
        }

        public static void Generate(this Type type)
        {
            artifactGenerated[type]--;
        }

        static ArtifactItemInfo()
        {
            artifactGenerated = new Dictionary<Type, int>();

            var itemType = typeof(Item);
            var items = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetTypes())
                                .Where(i => itemType.IsAssignableFrom(i)
                                    && !i.IsNestedPrivate
                                    && !i.IsAbstract)
                                .ToArray();
            var artifacts = from item in items
                            let a = (Artifact)item.GetCustomAttributes(typeof(Artifact), false).FirstOrDefault()
                            where a != null
                            select new { Item = item, Count = a.Count };

            foreach (var artifactInfo in artifacts)
            {
                artifactGenerated[artifactInfo.Item] = artifactInfo.Count;
            }
        }

        public static void Save(BinaryWriter bW)
        {
            bW.Write(artifactGenerated.Count);
            foreach (var key in artifactGenerated.Keys)
            {
                bW.Write(key.ToString());
                bW.Write(artifactGenerated[key]);
            }
        }

        public static void Load(BinaryReader bR)
        {
            var count = bR.ReadInt32();

            artifactGenerated = new Dictionary<Type, int>(count);

            for (int i = 0; i < count; i++)
            {
                var type = Type.GetType(bR.ReadString());
                artifactGenerated[type] = bR.ReadInt32();
            }
        }
    }
}
