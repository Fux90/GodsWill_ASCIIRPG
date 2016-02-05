using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class MenuUserControl : UserControl, IMenuViewer
    {
        private const float charSize = 10.0f;

        private int selectedIndex;
        private string[] menuLabels;

        public Font SelectedFont { get; private set; }
        public Color SelectedColor { get; private set; }
        public Color BarColor { get; private set; }

        public MenuUserControl()
        {
            InitializeComponent();
            menuLabels = new string[] { };

            this.DoubleBuffered = true;

            this.BackColor = Color.Black;
            this.ForeColor = Color.Black;
            this.SelectedColor = Color.Yellow;
            this.BarColor = Color.Blue;

            this.Font = new Font(FontFamily.GenericMonospace, charSize);
        }

        public void NotifyChangeSelection(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;
            this.Refresh();
        }

        public void NotifyLabels(string[] menuLabels)
        {
            this.menuLabels = menuLabels;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(menuLabels != null)
            {
                var g = e.Graphics;
                var posY = .0f;
                var posX = .0f;

                var normalBrush = new SolidBrush(ForeColor);
                var highlightBrush = new SolidBrush(SelectedColor);

                for (int i = 0; i < menuLabels.Length; i++)
                {
                    if(i == selectedIndex)
                    {
                        g.FillRectangle(new SolidBrush(BarColor),
                                        new RectangleF(new PointF(posX, posY),
                                                        new SizeF(this.Width, FontHeight)));
                        g.DrawString(menuLabels[i],
                                        Font,
                                        highlightBrush,
                                        new PointF(posX, posY));
                    }
                    else
                    {
                        g.DrawString(menuLabels[i],
                                        Font,
                                        normalBrush,
                                        new PointF(posX, posY));
                    }

                    posY += this.Font.Height;
                }
            }
        }
    }
}
