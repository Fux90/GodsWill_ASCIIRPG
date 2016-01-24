using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GodsWill_ASCIIRPG.Model
{
    public enum TargetType
    {
        Personal,
        AllEnemiesInRange,
        AllAlliesInRange,
        NumberOfTargets
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Target : Attribute
    {
        public TargetType TargetType { get; private set; }
        public int NumericParameter{ get; set; }

        public Target(TargetType targetType)
        {
            TargetType = targetType;
        }
    }

    public abstract class Spell
    {
        public static List<Type> All
        {
            get
            {
                var typeOfSpell = typeof(SpellBuilder);

                var ts =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where t.IsSubclassOf(typeOfSpell)
                where !t.IsAbstract
                select t;

                return ts.ToList<Type>();
            }
        }

        public string Name {  get { return this.GetType().Name.Clean(); } }

        public Pg.Level MinimumLevel
        {
            get
            {
                var prerequisite = this.GetType().GetCustomAttributes(typeof(Prerequisite), false);
                return prerequisite.Length > 0
                    ? ((Prerequisite)prerequisite[0]).MinimumLevel
                    : Pg.Level.Novice;
            }
        }
        private int StopForTurns
        {
            get
            {
                var m = (BlockSpellcasterFor[])this.GetType().GetCustomAttributes(typeof(BlockSpellcasterFor), false);
                return m.Length == 0 ? 0 : m[0].Turns;
            }
        }

        public bool IsFreeAction
        {
            get
            {
                return this.GetType().GetCustomAttributes(typeof(FreeAction), false).Length > 0;
            }
        }

        Animation animation;
        public ISpellcaster Launcher
        {
            get; private set;
        }

        public Spell(ISpellcaster launcher, Animation animation)
        {
            this.Launcher = launcher;
            this.animation = animation;
        }

        /// <summary>
        /// Called by character.
        /// Spell is forced to have one.
        /// It will call Launch(Atom, List[Atom], object),
        ///  organizing parameters properly 
        /// </summary>
        /// <param name="launcher"></param>
        /// <returns>True if turn consuming</returns>
        public abstract void Launch();

        /// <summary>
        /// The real behaviour of
        /// </summary>
        /// <param name="launcher"></param>
        /// <param name="targets"></param>
        /// <param name="parameters"></param>
        protected abstract void Effect(AtomCollection targets, object parameters);

        protected void Launch(AtomCollection targets, object parameters = null)
        {
            Launcher.BlockForTurns(this.StopForTurns);

            if(animation != null)
            {
                animation.Play();
            }

            Effect(targets, parameters);
        }
    }

    public abstract class SpellBuilder : Atom, Descriptionable
    {
        public ISpellcaster Caster { get; set; }
        public abstract string FullDescription { get; }

        public SpellBuilder()
            : base( "Spellbuilder",
                    "SB",
                    Color.White,
                    true,
                    false)
        {
        }

        public abstract string Name { get; }
        public abstract Target Target { get; }
        public abstract Spell Create(out bool issues);

        public abstract Type SpellToBuildType { get; }
        public abstract void SetTargets<T>(List<T> targets);
    }

    public abstract class SpellBuilder<T, SpellToBuild> : SpellBuilder
        where SpellToBuild : Spell
    {
        public List<T> Targets { get; protected set; }

        public SpellBuilder()
        {
            Targets = new List<T>();
        }

        public override string Name
        {
            get
            {
                return typeof(SpellToBuild).Name.Clean();
            }
        }

        public override Spell Create(out bool issues)
        {
            return (Spell)InnerCreate(out issues);
        }

        public abstract SpellToBuild InnerCreate(out bool issues);

        public override Target Target
        {
            get
            {
                var targetTypes = typeof(SpellToBuild).GetCustomAttributes(typeof(Target), false);
                return targetTypes.Length == 0 ? new Target(Model.TargetType.Personal) : (Target)targetTypes[0];
            }
        }

        public override Type SpellToBuildType
        {
            get
            {
                return typeof(SpellToBuild);
            }
        }

        public override bool Interaction(Atom interactor)
        {
            return false;
        }

        public override bool SatisfyRequisite(Pg player)
        {
            var satisfied = true;

            var requisites = typeof(SpellToBuild).GetCustomAttributes(typeof(Prerequisite), true);
            if (requisites.Length > 0)
            {
                var r = (Prerequisite)requisites[0];

                satisfied &= player.CurrentLevel >= r.MinimumLevel;
                satisfied &= player.Stats >= r.MinimumStats;
            }

            return satisfied;
        }
    }
}
