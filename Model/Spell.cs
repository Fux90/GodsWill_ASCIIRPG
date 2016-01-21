using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class Spell : Descriptionable
    {

        public string Name {  get { return this.GetType().Name.Clean(); } }
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

        public abstract string FullDescription { get; }

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
}
