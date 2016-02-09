using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
                                            Color color,
                                            Font font = null)
        {
            return new Label()
            {
                Name = name,
                AutoSize = false,
                Font = font == null ? control.Font : font,
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

        public static Direction Opposite(this Direction dir)
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

        public static Coord CoordFromDirection(this Coord ptFrom, Direction dir)
        {
            switch(dir)
            {
                case Direction.North:
                    return new Coord(ptFrom.X, ptFrom.Y - 1);
                case Direction.NorthEast:
                    return new Coord(ptFrom.X + 1, ptFrom.Y - 1);
                case Direction.East:
                    return new Coord(ptFrom.X + 1 , ptFrom.Y);
                case Direction.SouthEast:
                    return new Coord(ptFrom.X + 1, ptFrom.Y + 1);
                case Direction.South:
                    return new Coord(ptFrom.X, ptFrom.Y + 1);
                case Direction.SouthWest:
                    return new Coord(ptFrom.X - 1, ptFrom.Y + 1);
                case Direction.West:
                    return new Coord(ptFrom.X - 1, ptFrom.Y);
                case Direction.NorthWest:
                    return new Coord(ptFrom.X - 1, ptFrom.Y - 1);
                default:
                    return new Coord(ptFrom);
            }
        }

        public static bool SupportsSingleMsgListener(this Atom atom)
        {
            var result = false;
            var supported = new List<Type>
            {
                typeof(Item)
            };
            var atomType = atom.GetType();
            supported.ForEach(s => { result |= atomType.IsSubclassOf(s); });
            return result;
        }

        public static bool ToBool(this TernaryValue v)
        {
            return v == TernaryValue.Explored;
        }

        public static Coord Center(this Rectangle rect)
        {
            var x = (rect.Left + rect.Right) / 2;
            var y = (rect.Top + rect.Bottom) / 2;

            return new Coord(x, y);
        }

        public static TDelegate ToDelegate<TAlgorythms, TDelegate>(this string methodName)
        {
            var regex = new Regex(@"<get_(?<mName>.*)>.*");
            var match = regex.Match(methodName);
            var name = match.Groups["mName"].Value;
            var properties = typeof(TAlgorythms).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return (TDelegate)properties.Where(mI => mI.Name == name).Select(mI => mI.GetMethod).Single().Invoke(null, null);
        }

        public static string WeaponDescription(this Weapon._SpecialAttack obj)
        {
            var regex = new Regex(@"<get_(?<mName>.*)>.*");
            var match = regex.Match(obj.Method.Name);
            var name = match.Groups["mName"].Value;
            
            var desc =  (WeaponDescription)typeof(Weapon.WeaponSpecialAttacks)
                        .GetProperty(name)
                        .GetCustomAttributes(typeof(WeaponDescription), false).FirstOrDefault();

            return desc == null ? "" : desc.Description;
        }

        public static bool IsAttackable(this Atom atom)
        {
            if(atom.GetType().IsSubclassOf(typeof(Character))
                && ((Character)atom).AlliedTo == Allied.Pg)
            {
                return true;
            }

            return false;
        }

        public static bool AddOnce<T>(this List<T> lst, T item)
        {
            if(lst.Contains(item))
            {
                return false;
            }

            lst.Add(item);
            return true;
        }

        public static bool IsVowel(this char ch)
        {
            switch(ch)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                case 'A':
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                    return true;
                default:
                    return false;
            }
        }
    }
}
