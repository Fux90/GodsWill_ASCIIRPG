using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class GlowingAnimation : Animation
    {
        public GlowingAnimation(Atom atomToGlow, Color[] colors = null, int framePerColors = 3)
        {
            if(colors == null)
            {
                colors = new Color[]
                {
                    Color.DarkGray,
                    Color.Azure,
                };
            }
            var symbol = atomToGlow.Symbol;
            var pt = atomToGlow.Position;
             
            var fB = new FrameBuilder();
            foreach (Color color in colors)
            {
                fB.Clear();
                fB.AddFrameItem(symbol, color, pt);
                AddFrame(fB.Build());
            }
        }
    }
}
