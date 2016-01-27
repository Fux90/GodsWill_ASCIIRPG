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
    public partial class SingleMessageLogUserControl : UserControl, IAtomListener
    {
        private const float charSize = 10.0f;
        private readonly Font font = new Font(FontFamily.GenericMonospace, charSize);
        private string currentMsg = "";
        
        private float upperBorder
        {
            get
            {
                return (this.Height - charSize) / 2.0f;
            }
        }

        public SingleMessageLogUserControl()
        {
            InitializeComponent();

            this.Size = new Size();

            this.BackColor = Color.Black;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.DoubleBuffered = true;
        }

        public void NotifyMessage(Atom who, string msg)
        {
            currentMsg = msg;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawString(currentMsg, font, Brushes.White, new PointF(0, upperBorder));
        }
    }
}
