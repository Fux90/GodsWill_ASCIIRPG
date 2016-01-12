using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public static class Extensions
    {
        public delegate void GeneralAction();

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

        public static Label DockFillLabel(  this System.Windows.Forms.Control control, 
                                            string name, 
                                            Color color)
        {
            return new Label()
            {
                Name = name,
                AutoSize = false,
                Font = control.Font,
                ForeColor = color,
                BackColor = control.BackColor,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };
        }

        public static T ValueIfNotNullOrElse<T>(this T element, T value)
            where T : class
        {
            return element == null ? value : element;
        }

        public static T ValueIfNotNullOrElse<T>(this T? element, T value)
            where T : struct
        {
            return element == null ? value : (T)element;
        }

        public static void WaitForRefocus(this System.Windows.Forms.Control ctrl)
        {
            ctrl.WaitForRefocusThenDo(() => { });
        }

        public static void WaitForRefocusThenDo(this System.Windows.Forms.Control ctrl, GeneralAction action)
        {
            EventHandler waitBackpackClose = null;
            waitBackpackClose = (sender, e) =>
            {
                action();
                ctrl.Enter -= waitBackpackClose;
            };
            ctrl.Enter += waitBackpackClose;
        }

        public static Direction RandomDifferentFromThis(this Direction dir)
        {
            var numDirection = Enum.GetValues(typeof(Direction)).Length;
            var newDir = (int)dir;
            while ((newDir = (Dice.Throws(numDirection) - 1)) == (int)dir) ;
            return (Direction)newDir;
        }

        public static Direction TurnBack(this Direction dir)
        {
            var numDirection = Enum.GetValues(typeof(Direction)).Length;
            var numDirection2 = numDirection / 2;
            var newDir = (numDirection2 + (int)dir) % numDirection;
            return (Direction)newDir;
        }

        public static Direction DirectionFromOffset(this Coord ptFrom, Coord ptTo)
        {
            var ns = ptFrom - ptTo;
            var codDir = Math.Sign(ns.X) * 10 + Math.Sign(ns.Y);
            switch(codDir)
            {
                case 1: return Direction.North;
                case -9: return Direction.NorthEast;
                case -10: return Direction.East;
                case -11: return Direction.SouthEast;
                case -1: return Direction.South;
                case 9: return Direction.SouthWest;
                case 10: return Direction.West;
                case 11: return Direction.NorthWest;
                default: return Direction.North;
            }
        }
    }
}
