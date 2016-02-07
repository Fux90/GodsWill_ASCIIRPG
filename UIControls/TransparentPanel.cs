using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG.UIControls
{
    class TransparentPanel : Panel
    {
        public byte Transparency { get; set; }

        public TransparentPanel()
        {
            
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(Transparency, this.BackColor));//semi-transparent color.
            e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.Height));

            base.OnPaint(e);
        }
    }
}
