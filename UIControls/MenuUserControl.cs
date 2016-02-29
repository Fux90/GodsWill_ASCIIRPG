#define DEBUG_POSITION

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
        private const float charSize = 18.0f;
        private const float titleFontSize = 1.5f * charSize;
        private const float titlePadding = 2.0f;
        private const float separatorHeight = 1.0f;
        private const float paddingBetweenEntries = 4.0f;

        private const string helpString = "W: Up - S: Down - Enter: Select";
        private readonly Brush helpBrush = Brushes.Yellow;
         
        private int selectedIndex;
        private string[] menuLabels;
        private bool[] activeLabels;
        private string title;

        private Menu controlledMenu;

        public Font SelectedFont { get; private set; }
        public Font TitleFont { get; private set; }
        public Color TitleColor { get; private set; }
        public Color TitleBackColor { get; private set; }
        public Color InactiveColor { get; private set; }
        public Color SelectedColor { get; private set; }
        public Color InactiveSelectedColor { get; private set; }
        public Color BarColor { get; private set; }

        Brush normalBrush;
        Brush inactiveBrush;
        Brush highlightBrush;
        Brush inactiveHighlightBrush;
        Brush titleBrush;

        public MenuUserControl()
        {
            InitializeComponent();
            menuLabels = new string[] { };

            this.Size = new Size();
            this.MinimumSize = new Size();

            this.DoubleBuffered = true;

            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
            this.SelectedColor = Color.Yellow;
            this.InactiveColor = Color.Gray;
            this.InactiveSelectedColor = Color.DarkGray;
            this.BarColor = Color.Blue;

            this.Font = new Font(FontFamily.GenericMonospace, charSize);
            this.SelectedFont = new Font(FontFamily.GenericMonospace, charSize, FontStyle.Bold);
            this.TitleFont = new Font(FontFamily.GenericMonospace, titleFontSize);

            this.TitleColor = Color.Red;

            this.Dock = DockStyle.Fill;

            normalBrush = new SolidBrush(ForeColor);
            inactiveBrush = new SolidBrush(InactiveColor);
            highlightBrush = new SolidBrush(SelectedColor);
            inactiveHighlightBrush = new SolidBrush(InactiveSelectedColor);
            titleBrush = new SolidBrush(TitleColor);
        }

        public void NotifyChangeSelection(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;
            this.Refresh();
        }

        public void NotifyTitleChange(string title)
        {
            this.title = title;

            this.Refresh();
        }

        public void NotifyLabels(string[] menuLabels, bool[] activeLabels)
        {
            this.menuLabels = menuLabels;
            this.activeLabels = activeLabels;
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

        public void UnregisterAll()
        {
            this.controlledMenu = null;
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(menuLabels != null)
            {
                var g = e.Graphics;

                var titleLength = g.MeasureString(title, TitleFont).Width;
                var titlePos = new PointF(((float)this.Width - (float)titleLength) / 2.0f, titlePadding);

                g.DrawString(   title,
                                TitleFont,
                                titleBrush,
                                titlePos);

                var posY = titlePos.Y + 2 * titlePadding + this.FontHeight * 2;
                var posX = .0f;

#if DEBUG_POSITION
                var w2 = this.Width / 2.0f;
                g.DrawLine(Pens.Orange, new PointF(w2, .0f), new PointF(w2, posY));
#endif

                g.FillRectangle(Brushes.DarkGray, 
                                new RectangleF(new PointF(posX, posY), 
                                               new Size(this.Width, (int)separatorHeight)));

                posY += separatorHeight + paddingBetweenEntries;

                for (int i = 0; i < menuLabels.Length; i++)
                {
                    if(i == selectedIndex)
                    {
                        g.FillRectangle(new SolidBrush(BarColor),
                                        new RectangleF(new PointF(posX, posY),
                                                        new SizeF(this.Width, FontHeight)));

                        g.DrawString(menuLabels[i],
                                        SelectedFont,
                                        activeLabels[i] ? highlightBrush : inactiveHighlightBrush,
                                        new PointF(posX, posY));
                    }
                    else
                    {
                        g.DrawString(menuLabels[i],
                                        Font,
                                        activeLabels[i] ? normalBrush : inactiveBrush,
                                        new PointF(posX, posY));
                    }

                    posY += this.Font.Height + paddingBetweenEntries;
                }

#if FOCUSED_STRING
                g.DrawString(this.Focused.ToString(),
                                        Font,
                                        Brushes.Orange,
                                        new PointF(posX, posY));
#endif
                var len = g.MeasureString(helpString, Font).Width;
                var posHelp = new PointF((this.Width - len) / 2.0f, 
                                          this.Height - paddingBetweenEntries - FontHeight);

                g.FillRectangle(Brushes.DarkGray,
                                new RectangleF(new PointF(.0f, posHelp.Y - paddingBetweenEntries),
                                               new Size(this.Width, (int)separatorHeight)));

                g.DrawString(   helpString,
                                Font,
                                helpBrush,
                                posHelp);
            }
        }

        public void UpmostBring()
        {
            this.Show();
            this.BringToFront();
            this.Focus();

            var frm = this.Parent;
            while(frm != null && frm.GetType() != typeof(GameForm))
            {
                frm = frm.Parent;
            }

            ((GameForm)frm).ActiveControl = this;

            this.Refresh();
        }

        public void Close()
        {
            this.Hide();
        }
    }
}
