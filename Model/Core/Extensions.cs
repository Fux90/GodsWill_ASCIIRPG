﻿using System;
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
    }
}
