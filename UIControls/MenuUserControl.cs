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
    public partial class MenuUserControl : UserControl, IMenuViewer, MenuController
    {
        private const float charSize = 10.0f;

        private int selectedIndex;
        private string[] menuLabels;

        private Menu controlledMenu;

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

        public void Register(Menu menu)
        {
            this.controlledMenu = menu;
        }

        public void Unregister(Menu element)
        {
            if(this.controlledMenu == element)
            {
                this.controlledMenu = null;
            }
        }

        public void Notify(ControllerCommand cmd)
        {
            switch(cmd)
            {
                case ControllerCommand.Menu_PreviousItem:
                    controlledMenu.SelectPreviousItem();
                    break;
                case ControllerCommand.Menu_NextItem:
                    controlledMenu.SelectNextItem();
                    break;
                case ControllerCommand.Menu_ExecuteSelectItem:
                    controlledMenu.ExecuteSelected();
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W:
                    Notify(ControllerCommand.Menu_PreviousItem);
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Menu_NextItem);
                    break;
                case Keys.Enter:
                    Notify(ControllerCommand.Menu_ExecuteSelectItem);
                    break;
            }
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
