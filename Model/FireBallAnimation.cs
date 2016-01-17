using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class FireBallAnimation : Animation
    {
        public FireBallAnimation(Coord pt1, Coord pt2, Color color, string symbol = "*")
        {
            var line = new Line(pt1, pt2);

            var fB = new FrameBuilder();
            foreach (Coord pt in line)
            {
                fB.Clear();
                fB.AddFrameItem(symbol, color, pt);
                AddFrame(fB.Build());
            }
        }
    }
}
