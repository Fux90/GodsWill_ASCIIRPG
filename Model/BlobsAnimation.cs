using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public class BlobsAnimation : Animation
    {
        public BlobsAnimation(  List<Coord> centers, 
                                int maxRadius, 
                                Color color, 
                                string symbol = "*")
        {
            var fB = new FrameBuilder();

            for (int r = 2; r < Math.Max(3, maxRadius); r++)
            {
                var circles = centers.Select(c => new SimpleCircle(c, r));
                fB.Clear();

                foreach (var circle in circles)
                {
                    foreach (Coord pt in circle)
                    {
                        fB.AddFrameItem(symbol, color, pt);
                    }
                }

                AddFrame(fB.Build());
            }
        }
    }
}
