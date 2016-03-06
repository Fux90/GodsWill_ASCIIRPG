using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public abstract class ItemGenerator
    {
        public abstract Item GenerateRandom(Pg.Level level, Coord position, RarenessValue rareness);
        public abstract Type GeneratedType();
    }

    public abstract class ItemGenerator<T> : ItemGenerator
        where T : Item
    {
        public override Item GenerateRandom(Pg.Level level, Coord position, RarenessValue rareness = RarenessValue.Common)
        {
            return (Item)GenerateTypedRandom(level, position, rareness);
        }

        public override Type GeneratedType()
        {
            return typeof(T);
        }

        public abstract T GenerateTypedRandom(Pg.Level level, Coord position, RarenessValue rareness);
    }

    public static class ItemGenerators
    {
        static bool registered;
        static Dictionary<Type, ItemGenerator> generators;
        static Dictionary<Type, ItemGenerator> Generators
        {
            get
            {
                if(generators == null)
                {
                    generators = new Dictionary<Type, ItemGenerator>();
                }
                if(!registered)
                {
                    Register();
                }
                return generators;
            }
        }

        private static void Register()
        {
            registered = true;

            var typeOfGenerator = typeof(ItemGenerator);

            var ts =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            where t.IsSubclassOf(typeOfGenerator)
            where !t.IsAbstract
            select t;

            foreach (var type in ts)
            {
                Generators[type] = (ItemGenerator)Activator.CreateInstance(type);
            }
        }

        public static Item GenerateByBuilderType(   Type type, 
                                                    Pg.Level level = Pg.Level.Novice, 
                                                    Coord position = new Coord(),
                                                    RarenessValue rareness = RarenessValue.Common)
        {

            return Generators.ContainsKey(type)
                ? Generators[type].GenerateRandom(level, position, rareness)
                : null;
        }
    }
}
