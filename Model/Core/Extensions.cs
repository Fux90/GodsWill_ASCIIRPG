using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public static class Extensions
    {
        public static PointF Offset(this PointF pt1, float dX = 0, float dY = 0)
        {
            return new PointF(pt1.X + dX, pt1.Y + dY);
        }

        public static int ModifierOfStat(this Stats stats, StatsType stat)
        {
            return (int)Math.Floor((stats[stat] - 10.0) / 2.0);
        }

        public static int ModifierOfStat(this int stat)
        {
            return (int)Math.Floor((stat - 10.0) / 2.0);
        }

        public static string Clean(this string str)
        {
            return str.Replace("_", "");
        }
    }
}
