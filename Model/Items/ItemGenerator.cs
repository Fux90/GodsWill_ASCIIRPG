using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public abstract class ItemGenerator
    {
        public abstract Item GenerateRandom(Pg.Level level);
    }

    public abstract class ItemGenerator<T> : ItemGenerator
        where T : Item
    {
        public override Item GenerateRandom(Pg.Level level)
        {
            return (Item)GenerateTypedRandom(level);
        }

        public abstract T GenerateTypedRandom(Pg.Level level);
    }

    public static class ItemGenerators
    {
        static Dictionary<Type, ItemGenerator> generators;
        static Dictionary<Type, ItemGenerator> Generators
        {
            get
            {
                if(generators == null)
                {
                    generators = new Dictionary<Type, ItemGenerator>();
                }
                return generators;
            }
        }

        public static void Register(ItemGenerator generator)
        {
            Generators[generator.GetType()] = generator;
        }

        public static Item GenerateByType(Type type, Pg.Level level = Pg.Level.Novice)
        {

            return Generators.ContainsKey(type)
                ? Generators[type].GenerateRandom(level)
                : null;
        }
    }
}
