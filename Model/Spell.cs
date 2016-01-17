using GodsWill_ASCIIRPG.Model.Core;
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
        
        Animation animation;
        public Atom Launcher
        {
            get; private set;
        }

        public abstract string FullDescription { get; }

        public Spell(Atom launcher, Animation animation)
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
            if(animation != null)
            {
                animation.Play();
            }

            Effect(targets, parameters);
        }
    }
}
