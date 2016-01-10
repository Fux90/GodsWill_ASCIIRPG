using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.View;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class LogUserControl : UserControl, IAtomListener
    {
        public const float FontSize = 10.0f;
        private List<LogRow> rows;

        public LogUserControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Green;
            this.BorderStyle = BorderStyle.FixedSingle;
            rows = new List<LogRow>();
        }

        public void AppendText(LogRow row)
        {
            rows.Add(row);
        }

        public void NotifyMessage(Atom who, string msg)
        {
            //TODO: different colors, given different actions/monster
            rows.Add(new LogRow(String.Format("[{0}]: {1}", who.Name, msg))
            {
                Color = who.Color,
            });
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var numVisualizedRows = (int)Math.Ceiling(this.Height / (FontSize + 2.0f));
            var startIndex = 0;
            if (rows.Count >= numVisualizedRows)
            {
                startIndex = rows.Count - numVisualizedRows + 1;
            }

            var pos = new PointF(0.0f, 0.0f);
            for (int ixR = startIndex; ixR < rows.Count; ixR++)
            {
                g.DrawString(   rows[ixR].Message,
                                rows[ixR].Font,
                                rows[ixR].Brush,
                                pos);
                pos.Y += FontSize;
            }
        }
    }

    public struct LogRow
    {
        public Font Font { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get { return new SolidBrush(Color); } }
        public string Message { get; set; }

        public LogRow(string message)
        {
            Font = new Font(FontFamily.GenericMonospace, LogUserControl.FontSize);
            Color = Color.White;
            Message = message;
        }
    }
}
