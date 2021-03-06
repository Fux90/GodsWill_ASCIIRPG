﻿using GodsWill_ASCIIRPG.Main;
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
        public readonly Size InitSize = new Size(909, 569);

        TableLayoutPanel tblGameScreen;

        MenuUserControl mainMenuViewer;

        public GameForm()
        {
            this.Name = MyName;

            InitializeComponent();

            tblGameScreen = new TableLayoutPanel();
            tblGameScreen.Size = new Size();

            var singleMsgConsole = new SingleMessageLogUserControl();
            var backpackControl = new BackpackUserControl(tblGameScreen);
            var spellbookControl = new SpellbookUserControl(tblGameScreen);
            var mapViewControl = new MapUserControl(this, 
                                                    backpackControl, 
                                                    spellbookControl, 
                                                    singleMsgConsole);
            var characterSheet = new CharacterSheetUserControl();
            var logConsole = new LogUserControl();

            var merchantControl = new MerchantUserControl(tblGameScreen);

            tblGameScreen.Dock = DockStyle.Fill;

            tblGameScreen.RowStyles.Clear();
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Percent, 0.75F));
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 30.0f));
            tblGameScreen.RowStyles.Add(new RowStyle(SizeType.Percent, 0.25F));
            tblGameScreen.BackColor = Color.Beige;
            tblGameScreen.MinimumSize = new Size();

            var tblMapAndSheet = new TableLayoutPanel();
            tblMapAndSheet.Size = new Size();
            tblMapAndSheet.ColumnStyles.Clear();
            tblMapAndSheet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.40F));
            tblMapAndSheet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.60F));
            tblMapAndSheet.BackColor = Color.Beige;
            tblMapAndSheet.Margin = new Padding(0);
            tblMapAndSheet.MinimumSize = new Size();
            
            tblMapAndSheet.Controls.Add(mapViewControl, 0, 0);
            mapViewControl.Dock = DockStyle.Fill;
            tblMapAndSheet.Controls.Add(characterSheet, 1, 0);
            characterSheet.Dock = DockStyle.Fill;
            //var cellPos = tblMapAndSheet.GetCellPosition(mapViewControl);
            //int width = tblMapAndSheet.GetColumnWidths()[cellPos.Column];
            //mapViewControl.Size = new Size(20, 20);

            tblGameScreen.Controls.Add(tblMapAndSheet, 0, 0);
            tblMapAndSheet.Dock = DockStyle.Fill;

            tblGameScreen.Controls.Add(singleMsgConsole, 0, 1);
            singleMsgConsole.Dock = DockStyle.Fill;

            tblGameScreen.Controls.Add(logConsole, 0, 2);
            logConsole.Dock = DockStyle.Fill;

            backpackControl.Hide();
            spellbookControl.Hide();
            merchantControl.Hide();

            this.Controls.Add(tblGameScreen);
            this.Controls.Add(backpackControl);
            this.Controls.Add(spellbookControl);
            this.Controls.Add(merchantControl);

            this.Text = GameName;

            mainMenuViewer = new MenuUserControl();
            this.Controls.Add(mainMenuViewer);

            this.Size = InitSize;

            Game.Current.Init(  pgController: mapViewControl,
                                aiController: mapViewControl,
                                merchantController: merchantControl,
                                mapViewer: mapViewControl,
                                singleMsgListener: singleMsgConsole,
                                animationViewer: mapViewControl,
                                pgViewers: new List<IPgViewer>() { mapViewControl },
                                atomListeners: new List<IAtomListener> { logConsole },
                                sheetViews: new List<ISheetViewer> { characterSheet },
                                backpackViewers: new List<IBackpackViewer> { backpackControl },
                                spellbookViewers: new List<ISpellbookViewer> { spellbookControl },
                                secondarySpellsViewers: new List<IAtomListener> { singleMsgConsole },
                                merchantViewers: new List <IMerchantViewer> { merchantControl },
                                mainMenuViewer: mainMenuViewer,
                                mainMenuController: mainMenuViewer);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnActivated(e);

            Game.Current.InitialMenu();

            //Game.Current.GameInitialization(mapViewControl, 
            //                                mapViewControl, 
            //                                mapViewControl,
            //                                singleMsgConsole,
            //                                mapViewControl,
            //                                new List<IAtomListener> { logConsole },
            //                                new List<ISheetViewer> { characterSheet },
            //                                new List<IBackpackViewer> { backpackControl },
            //                                new List<ISpellbookViewer> { spellbookControl },
            //                                new List<IAtomListener> { singleMsgConsole });
            //Game.Current.RunGame(); 
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MessageBox.Show("Bye");
        }
    }
}
