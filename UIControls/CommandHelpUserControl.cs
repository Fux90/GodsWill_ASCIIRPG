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
    public partial class CommandHelpUserControl : UserControl, ICommandHelpViewer
    {
        // Key + CombinationOf([Ctrl|Shift|Alt]) -> Command string description
        readonly private Dictionary<Keys, ModifierCommands[]> commands =
            new Dictionary<Keys, ModifierCommands[]>()
            {
                {
                    Keys.W,
                    new ModifierCommands[]
                    {
                        new ModifierCommands(null, "Move up")
                    }
                },
                {
                    Keys.A,
                    new ModifierCommands[]
                    {
                        new ModifierCommands(null, "Move left")
                    }
                },
                {
                    Keys.S,
                    new ModifierCommands[]
                    {
                        new ModifierCommands(null, "Move down")
                    }
                },
                {
                    Keys.D,
                    new ModifierCommands[]
                    {
                        new ModifierCommands(null, "Move right")
                    }
                },
            };

        private List<CommandLine> CommandStrings;

        private const float charSize = 12.0f;
        private const float titleFontSize = 1.5f * charSize;
        private const float titlePadding = 2.0f;
        private const float separatorHeight = 1.0f;
        private const float paddingBetweenEntries = 4.0f;

        System.Windows.Forms.Control container;

        private Brush titleBrush = Brushes.Red;
        private readonly Brush helpBrush = Brushes.Yellow;
        private readonly Brush inactiveHelpBrush = Brushes.DarkGray;
        private readonly Brush normalBrush = Brushes.White;
        private Brush keyBrush = Brushes.Orange;

        private string title = "COMMANDS";
        private const string leftPageString = "< A";
        private const string rightPageString = "D >";
        private const string helpString = " - Esc Exit - ";

        private int currentPageIndex = 0;
        private int maxPageIndex
        {
            get
            {
                return CommandStrings.Count / numLinesPerPage;
            }
        }

        private int firstLineIndex
        {
            get
            {
                return currentPageIndex * numLinesPerPage;
            }
        }
        private int numLinesPerPage
        {
            get
            {
                return (int)Math.Max(   1,
                                        Math.Ceiling((this.Height - ( 2 * titlePadding 
                                                                    + 3 * FontHeight 
                                                                    + 2 * separatorHeight
                                                                    + 3 * paddingBetweenEntries)) 
                                                    / (FontHeight + paddingBetweenEntries)));
            }
        }

        public Font TitleFont { get; private set; }

        public CommandHelpUserControl(System.Windows.Forms.Control container)
        {
            InitializeComponent();
            this.container = container;

            container.Controls.Add(this);
            this.Dock = DockStyle.Fill;

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, charSize);
            this.TitleFont = new Font(FontFamily.GenericMonospace, titleFontSize);

            KeyUp += CommandHelpUserControl_KeyUp;
            Paint += CommandHelpUserControl_Paint;
            Resize += CommandHelpUserControl_Resize;
            BuildCommandStrings();
        }

        private void CommandHelpUserControl_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void BuildCommandStrings()
        {
            CommandStrings = new List<CommandLine>();

            foreach (var key in commands.Keys)
            {
                var modToCommand = commands[key];

                foreach (var modLst in modToCommand)
                {
                    var keys = new StringBuilder(key.ToString());
                    var mods = modLst.Mods;

                    if (mods != null && mods.Length != 0)
                    {
                        for (int i = 0; i < mods.Length; i++)
                        {
                            keys.AppendFormat("{0}+{1}", mods[i], keys.ToString());
                        }
                    }
                    keys.Append(":");

                    CommandStrings.Add(
                        new CommandLine(
                            keys: keys.ToString(),
                            command: modLst.Command
                        ));
                }
            }
        }

        private void CommandHelpUserControl_Paint(object sender, PaintEventArgs e)
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

            g.FillRectangle(Brushes.DarkGray,
                            new RectangleF(new PointF(posX, posY),
                                            new Size(this.Width, (int)separatorHeight)));

            posY += separatorHeight + paddingBetweenEntries;

            var first = firstLineIndex;
            var last = Math.Min(CommandStrings.Count, first + numLinesPerPage);
            //for(int i = 0; i< CommandStrings.Count; i++)
            for (int i = first; i < last; i++)
            {
                var line = CommandStrings[i];
                var keys = line.Keys;
                
                posX = .0f;
                    g.DrawString(   keys,
                                    Font,
                                    keyBrush,
                                    new PointF(posX, posY));

                posX += g.MeasureString(keys, TitleFont).Width;

                g.DrawString(line.Command,
                            Font,
                            normalBrush,
                            new PointF(posX, posY));

                posY += this.Font.Height + paddingBetweenEntries;
            }

            var lenLeft = g.MeasureString(leftPageString, Font).Width;
            var lenRight = g.MeasureString(rightPageString, Font).Width;
            var len = g.MeasureString(helpString, Font).Width;

            var posHelp = new PointF((this.Width - (lenLeft + lenRight + len)) / 2.0f,
                                        this.Height - paddingBetweenEntries - FontHeight);

            g.FillRectangle(Brushes.DarkGray,
                            new RectangleF(new PointF(.0f, posHelp.Y - paddingBetweenEntries),
                                            new Size(this.Width, (int)separatorHeight)));

            g.DrawString(   leftPageString,
                            Font,
                            currentPageIndex > 0 ? helpBrush : inactiveHelpBrush,
                            posHelp);
            posHelp.X += lenLeft;
            g.DrawString(   helpString,
                            Font,
                            helpBrush,
                            posHelp);
            posHelp.X += len;
            g.DrawString(   rightPageString,
                            Font,
                            currentPageIndex < maxPageIndex ? helpBrush : inactiveHelpBrush,
                            posHelp);

        }

        private void CommandHelpUserControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.A:
                    currentPageIndex = Math.Max(currentPageIndex - 1, 0);
                    this.Refresh();
                    break;
                case Keys.D:
                    currentPageIndex = Math.Min(currentPageIndex + 1, maxPageIndex);
                    this.Refresh();
                    break;
            }
        }

        public void UpmostBring()
        {
            this.Show();
            this.BringToFront();

            var frm = this.Parent;
            while (frm != null && frm.GetType() != typeof(GameForm))
            {
                frm = frm.Parent;
            }

            ((GameForm)frm).ActiveControl = this;

            this.Refresh();
        }

        public void Close()
        {
            this.Hide();
            this.container.Controls.Remove(this);
            this.Dispose();
        }

        private class ModifierCommands
        {
            public Keys[] Mods { get; private set; }
            public string Command { get; private set; }

            public ModifierCommands(Keys[] mods, string command)
            {
                Mods = mods;
                Command = command;
            }
        }

        private class CommandLine
        {
            public string Keys { get; private set; }
            public string Command { get; private set; }

            public CommandLine(string keys, string command)
            {
                Keys = keys;
                Command = command;
            }
        }
    }
}
