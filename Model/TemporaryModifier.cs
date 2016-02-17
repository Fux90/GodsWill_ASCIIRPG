using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ApplyableTo : Attribute
    {
        public TemporaryModifier.ModFor What { get; private set; }

        public ApplyableTo(TemporaryModifier.ModFor modFor)
        {
            What = modFor;
        }
    }

    [Serializable]
    public class TemporaryModifierCollection
    {
        List<TemporaryModifier> modifiers;

        public TemporaryModifierCollection()
        {
            modifiers = new List<TemporaryModifier>();
        }

        /// <summary>
        /// Ages temp modifiers
        /// </summary>
        /// <returns>How many mods have expired</returns>
        public int PassedTurn()
        {
            var prevCount = modifiers.Count;
            modifiers.ForEach(mod => mod.PassedTurn());
            modifiers.RemoveAll(mod => mod.TimeToLive == 0);
            return prevCount - modifiers.Count;
        }

        public delegate T summingTMethod<T>(T e1, T e2);
        public T GetBonus<T>(TemporaryModifier.ModFor ApplyableFor,
                                summingTMethod<T> sumMethod,
                                T baseBonus = default(T))
        {
            var modT = modifiers.Where(mod => { var attr = mod.GetType().GetCustomAttributes(typeof(ApplyableTo));
                                                return attr.Count( a => ((ApplyableTo)a).What == ApplyableFor) > 0; })
                                .Where(mod => mod.GetType()
                                                .IsSubclassOf(typeof(TemporaryModifier<T>)))
                                                .Select(mod => (TemporaryModifier<T>)mod)
                                                .ToList();
            T totalBonus = baseBonus;
            modT.Where(mod => mod.What == ApplyableFor).ToList()
                .ForEach(mod => totalBonus = sumMethod(totalBonus, mod.Bonus));

            return totalBonus;
        }

        public void AddTemporaryModifier(TemporaryModifier mod)
        {
            modifiers.Add(mod);
        }
    }

    [Serializable]
    public abstract class TemporaryModifier : TypeQueryable
    {
        public enum ModFor
        {
            CA,
            CASpecial,
            Stats,
            ListenPerception,
            SpotPerception
        }

        public int TimeToLive { get; private set; }
        public ModFor What { get; private set; }

        protected TemporaryModifier(int ttl, ModFor applyTo)
        {
            TimeToLive = ttl;
            What = applyTo;
        }

        public void PassedTurn()
        {
            TimeToLive--;
        }
    }

    public abstract class TemporaryModifier<T> : TemporaryModifier
    {
        public TemporaryModifier(int ttl, ModFor applyTo, T bonus)
            : base(ttl, applyTo)
        {
            Bonus = bonus;
        }

        public string TypeOfModifier
        {
            get
            {
                //return this.GetType().Name.Clean();
                return this.Type.Name.Clean();
            }
        }

        public T Bonus
        {
            get; private set;
        }

        public override string ToString()
        {
            return String.Format(   "{0} {1} to {2}", 
                                    Bonus.ToString(),
                                    this.TypeOfModifier,
                                    this.What);
        }
    }
}
