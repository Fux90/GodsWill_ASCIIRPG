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
        readonly private CommandLine[] commands =
            new CommandLine[]
            {
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.F1},
                    Mods = new ModifierCommands(
                        null,
                        "Help")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.W, Keys.A, Keys.S, Keys.D },
                    Mods = new ModifierCommands(null, "Move up/left/down/right")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.S },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Control },
                        "Save game")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.S },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Shift },
                        "Open spellbook")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.G },
                    Mods = new ModifierCommands(
                        null,
                        "Grab from the ground")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.I },
                    Mods = new ModifierCommands(
                        null,
                        "Open backpack")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.P },
                    Mods = new ModifierCommands(
                        null,
                        "Put on armor")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.P },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Control},
                        "Put off armor")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.B },
                    Mods = new ModifierCommands(
                        null,
                        "Handle shield")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.B },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Control},
                        "Unhandle shield")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.H },
                    Mods = new ModifierCommands(
                        null,
                        "Handle weapon")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.H },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Control},
                        "Unhandle weapon")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.H },
                    Mods = new ModifierCommands(
                        new Keys[] { Keys.Shift},
                        "Activate/Deactivate weapon special power")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.U },
                    Mods = new ModifierCommands(
                        null,
                        "Use object")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.K },
                    Mods = new ModifierCommands(
                        null,
                        "Kneel to pray")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.X },
                    Mods = new ModifierCommands(
                        null,
                        "Explore")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.Enter, Keys.Add },
                    Mods = new ModifierCommands(
                        null,
                        "Confirm single/multiple selection")
                },
                new CommandLine()
                {
                    Keys = new Keys[]{ Keys.Home },
                    Mods = new ModifierCommands(
                        null,
                        "Back to Main Menu")
                },
            };

        private List<CommandLineString> CommandStrings;

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
            CommandStrings = new List<CommandLineString>();

            foreach (CommandLine command in commands)
            {
                var _keys = command.Keys;
                var modToCommand = command.Mods;

                var keyStr = new StringBuilder(_keys[0].ToString());

                for (int i = 1; i < _keys.Length; i++)
                {
                    keyStr.AppendFormat("/{0}", _keys[i]);
                }

                var keys = new StringBuilder();
                var mods = modToCommand.Mods;

                if (mods != null && mods.Length != 0)
                {
                    keys.AppendFormat("{0}+", mods[0]);

                    for (int i = 1; i < mods.Length; i++)
                    {
                        keys.AppendFormat("{0}+", mods[i]);
                    }
                }
                keys.AppendFormat("{0}:", keyStr.ToString());

                CommandStrings.Add(
                    new CommandLineString(
                        keys: keys.ToString(),
                        command: modToCommand.Command
                    ));
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

                posX += g.MeasureString(keys, Font).Width;

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
            public Keys[] Keys {get;set;}
            public ModifierCommands Mods { get; set; }
        }

        private class CommandLineString
        {
            public string Keys { get; private set; }
            public string Command { get; private set; }

            public CommandLineString(string keys, string command)
            {
                Keys = keys;
                Command = command;
            }
        }
    }
}
