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
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class CharacterSheetUserControl : UserControl, ISheetViewer
    {
        private const float charSize = 10.0f;
        private const float padding = charSize;

        private const string _lblName = "lblName";
        private const string _lblLevel = "lblLevel";
        private const string _lblXP = "lblXP";
        private const string _lblPf = "lblPf";
        private const string _lblHunger = "lblHunger";
        private const string _lblCA = "lblCA";
        private const string _lblCASpecial = "lblCASpecial";
        private const string _lblArmor = "lblArmor";
        private const string _lblShield = "lblShield";
        private const string _lblWeapon = "lblWeapon";
        private const string _lblGold = "lblGold";

        TableLayoutPanel tblMainPanelLayout;
        TableLayoutPanel tblPanelNameAndLevel;
        TableLayoutPanel tblPfAndHunger;
        TableLayoutPanel tblCAs;
        TableLayoutPanel tblStats;

        public CharacterSheetUserControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;

            this.Font = new Font(FontFamily.GenericMonospace, charSize);

            tblMainPanelLayout = new TableLayoutPanel();
            tblMainPanelLayout.RowStyles.Clear();
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 6.0f)); // Name and Level
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // XP
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // Pf and hunger
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // CAs
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0f));// Stats
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 18.0f));// Armor
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 18.0f));// Shield
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 18.0f));// Weapon
            tblMainPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f));// Gold

            tblPanelNameAndLevel = CreateColumns(new float[] { 60.0f, 40.0f });
            tblPanelNameAndLevel.Controls.Add(this.DockFillLabel(_lblName, Color.Red), 0, 0);
            tblPanelNameAndLevel.Controls.Add(this.DockFillLabel(_lblLevel, Color.DarkOrchid), 1, 0);

            tblPfAndHunger = CreateColumns(new float[] { 50.0f, 50.0f });
            tblPfAndHunger.Controls.Add(this.DockFillLabel(_lblPf, Color.White), 0, 0);
            tblPfAndHunger.Controls.Add(this.DockFillLabel(_lblHunger, Color.White), 1, 0);

            tblCAs = CreateColumns(new float[] { 50.0f, 50.0f });
            tblCAs.Controls.Add(this.DockFillLabel(_lblCA, Color.White), 0, 0);
            tblCAs.Controls.Add(this.DockFillLabel(_lblCASpecial, Color.White), 1, 0);

            var numStats = Stats.AllStats.Length;
            var numRows = numStats / 2;
            var perc = 100.0f / numRows;

            tblStats = CreateRows((float[])Enumerable.Repeat(perc, numRows).ToArray());
            tblStats.Margin = new Padding(0);
            for (int i = 0; i < numRows; i++)
            {
                var cols = CreateColumns(new float[] { 25.0f, 25.0f, 25.0f, 25.0f });
                tblStats.Controls.Add(cols, 0, i);
                cols.Controls.Add(this.DockFillLabel("lbl_c0r" + i.ToString(), Color.Blue), 0, 0);
                cols.Controls.Add(this.DockFillLabel("lbl_c1r" + i.ToString(), Color.White), 1, 0);
                cols.Controls.Add(this.DockFillLabel("lbl_c0r" + i.ToString(), Color.Blue), 2, 0);
                cols.Controls.Add(this.DockFillLabel("lbl_c1r" + i.ToString(), Color.White), 3, 0);
                cols.Margin = new Padding(0);
                cols.Dock = DockStyle.Fill;
            }
            
            tblMainPanelLayout.Controls.Add(tblPanelNameAndLevel, 0, 0);
            tblMainPanelLayout.Controls.Add(this.DockFillLabel(_lblXP, Color.White), 0, 1);
            tblMainPanelLayout.Controls.Add(tblPfAndHunger, 0, 2);
            tblMainPanelLayout.Controls.Add(tblCAs, 0, 3);
            tblMainPanelLayout.Controls.Add(tblStats, 0, 4);
            tblMainPanelLayout.Controls.Add(this.DockFillLabel(_lblArmor, Color.DarkRed), 0, 5);
            tblMainPanelLayout.Controls.Add(this.DockFillLabel(_lblShield, Color.Brown), 0, 6);
            tblMainPanelLayout.Controls.Add(this.DockFillLabel(_lblWeapon, Color.Gray), 0, 7);
            tblMainPanelLayout.Controls.Add(this.DockFillLabel(_lblGold, Color.Gold), 0, 8);

            this.Controls.Add(tblMainPanelLayout);

            tblMainPanelLayout.Dock = DockStyle.Fill;
            tblPanelNameAndLevel.Dock = DockStyle.Fill;
            tblPfAndHunger.Dock = DockStyle.Fill;
            tblCAs.Dock = DockStyle.Fill;
            tblStats.Dock = DockStyle.Fill;
        }

        private TableLayoutPanel CreateColumns(float[] percentages)
        {
            var tblPanel = new TableLayoutPanel();
            tblPanel.ColumnStyles.Clear();
            foreach (var perc in percentages)
            {
                if (perc == -1.0f)
                {
                    tblPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, .1f));
                }
                else
                {
                    tblPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, perc));
                }
            }

            tblPanel.Margin = new Padding(0);
            return tblPanel;
        }

        private TableLayoutPanel CreateRows(float[] percentages)
        {
            var tblPanel = new TableLayoutPanel();
            tblPanel.RowStyles.Clear();
            foreach (var perc in percentages)
            {
                tblPanel.RowStyles.Add(new RowStyle(SizeType.Percent, perc));
            }

            return tblPanel;
        }

        private void setTextAndRefresh(System.Windows.Forms.Control ctrl, string txt)
        {
            ctrl.Text = txt;
            ctrl.Refresh();
        }

        public void NotifyArmor(Armor armor)
        {
            setTextAndRefresh(tblMainPanelLayout.Controls[_lblArmor], armor.ToString());
        }

        public void NotifyDefences(int CA, int SpecialCA)
        {
            setTextAndRefresh(tblCAs.Controls[_lblCA], String.Format("CA: {0,3}", CA));
            setTextAndRefresh(tblCAs.Controls[_lblCASpecial], String.Format("Special: {0,3}", SpecialCA));
        }

        public void NotifyHunger(int hunger)
        {
            setTextAndRefresh(tblPfAndHunger.Controls[_lblHunger], String.Format("Hunger: {0,3}", hunger));
        }

        public void NotifyLevel(Pg.Level level, God god)
        {
            setTextAndRefresh(  tblPanelNameAndLevel.Controls[_lblLevel], 
                                String.Format("[{0}{1}]", 
                                    level.ToString(), 
                                    god != null
                                    ? String.Format(" of {0}", god.Name)
                                    : ""));
        }

        public void NotifyName(string name)
        {
            setTextAndRefresh(tblPanelNameAndLevel.Controls[_lblName], name);
            
            var size = (float)tblPanelNameAndLevel.Controls[_lblName].Text.Length * Font.SizeInPoints + padding;
            tblPanelNameAndLevel.ColumnStyles[0].SizeType = SizeType.Absolute;
            tblPanelNameAndLevel.ColumnStyles[0].Width = size;
        }

        public void NotifyGold(int currentGold)
        {
            setTextAndRefresh(tblMainPanelLayout.Controls[_lblGold], String.Format("$: {0}", currentGold));
        }

        public void NotifyHp(int currentHp, int maximumHp)
        {
            setTextAndRefresh(tblPfAndHunger.Controls[_lblPf], String.Format("HP: {0,3}/{1,3}", currentHp, maximumHp));
        }

        public void NotifyShield(Shield shield)
        {
            setTextAndRefresh(tblMainPanelLayout.Controls[_lblShield], shield.ToString());
        }

        public void NotifyStat(StatsType stat, int value)
        {
            var col = ((int)stat) % 2;
            var row = ((int)stat) / 2;

            var rowTbl = (TableLayoutPanel)tblStats.GetControlFromPosition(0, row);
            var col2 = 2 * col;
            var lblNameStat = rowTbl.GetControlFromPosition(col2, 0);
            var lblValueStat = rowTbl.GetControlFromPosition(col2 + 1, 0);
            setTextAndRefresh(  lblNameStat, 
                                String.Format(  "{0}", 
                                                stat.ToString()));
            setTextAndRefresh(  lblValueStat,
                                String.Format("{0, 3} ({1})",
                                                value,
                                                value.ModifierOfStat().ToString("+#;-#;0")));
        }

        public void NotifyWeapon(Weapon weapon)
        {
            setTextAndRefresh(tblMainPanelLayout.Controls[_lblWeapon], weapon.ToString());
        }

        public void NotifyXp(int currentXp, int nextXp)
        {
            setTextAndRefresh(tblMainPanelLayout.Controls[_lblXP], String.Format("XP: {0,3}/{1,3}", currentXp, nextXp));
        }
    }
}
