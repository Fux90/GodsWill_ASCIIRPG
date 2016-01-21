using GodsWill_ASCIIRPG.Main;
using GodsWill_ASCIIRPG.UIControls;
using GodsWill_ASCIIRPG.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG
{
    public partial class GameForm : Form
    {
        public const string MyName = "MainForm";
        public const string GameName = "GOD'S WILL";

        Game game;
        TableLayoutPanel tblGameScreen;

        public GameForm()
        {
            this.Name = MyName;

            InitializeComponent();

            tblGameScreen = new TableLayoutPanel();

            var singleMsgConsole = new SingleMessageLogUserControl();
            var backpackControl = new BackpackUserControl(tblGameScreen);
            var mapViewControl = new MapUserControl(this, backpackControl, singleMsgConsole);
            var characterSheet = new CharacterSheetUserControl();
            var logConsole = new LogUserControl();            

            tblGameScreen.Dock = DockStyle.Fill;
            tblGameScreen.RowStyles.Clear();
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Percent, 0.75F));
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 30.0f));
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Percent, 0.25F));
            tblGameScreen.BackColor = Color.Beige;

            var tblMapAndSheet = new TableLayoutPanel();
            tblMapAndSheet.ColumnStyles.Clear();
            tblMapAndSheet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.60F));
            tblMapAndSheet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.40F));
            tblMapAndSheet.BackColor = Color.Beige;
            tblMapAndSheet.Margin = new Padding(0);

            tblMapAndSheet.Controls.Add(mapViewControl, 0, 0);
            mapViewControl.Dock = DockStyle.Fill;
            tblMapAndSheet.Controls.Add(characterSheet, 1, 0);
            characterSheet.Dock = DockStyle.Fill;

            tblGameScreen.Controls.Add(tblMapAndSheet, 0, 0);
            tblMapAndSheet.Dock = DockStyle.Fill;

            tblGameScreen.Controls.Add(singleMsgConsole, 0, 1);
            singleMsgConsole.Dock = DockStyle.Fill;

            tblGameScreen.Controls.Add(logConsole, 0, 2);
            logConsole.Dock = DockStyle.Fill;

            backpackControl.Hide();

            this.Controls.Add(tblGameScreen);
            this.Controls.Add(backpackControl);

            this.Text = GameName;

            Game.Current.Init();
            Game.Current.InitialMenu(   new List<IAtomListener> { logConsole },
                                        new List<ISheetViewer> { characterSheet },
                                        new List<IBackpackViewer> { backpackControl });
            Game.Current.GameInitialization(mapViewControl, 
                                            mapViewControl, 
                                            mapViewControl,
                                            singleMsgConsole,
                                            mapViewControl);
            //Game.Current.RunGame();
        }

        protected override void OnLoad(EventArgs e)
        {
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MessageBox.Show("Bye");
        }
    }
}
