using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    /// <summary>
    /// Animation Viewer
    /// </summary>
    public interface IAnimationViewer
    {
        /// <summary>
        /// Play a single frame of an animation
        /// </summary>
        /// <param name="animation">Animation to be played</param>
        void PlayAnimation(Animation animation);
    }
}
