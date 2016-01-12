using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class BackpackUserControl : UserControl, BackpackController, IBackpackViewer
    {
        private const int PaddingValue = 4;

        Backpack controlledBackpack;
        PagedListUserControl<Item> itemList;
        Label lblDescription;
        Label lblTitle;

        TableLayoutPanel gamePanel;

        private int initialWidth;
        private int initialHeight;
        private float initialFontSize;

        public int SelectedIndex
        {
            get
            {
                var ix = itemList.SelectedIndex;
                ValidIndex = false;
                return ix;
            }
        }
        public bool ValidIndex { get; private set; }

        public bool Opened { get; private set; }

        public BackpackUserControl(TableLayoutPanel gamePanel)
        {
            InitializeComponent();

            this.gamePanel = gamePanel;
            this.BackColor = Color.Black;

            var tblPanel = new TableLayoutPanel();
            tblPanel.BackColor = Color.LightGray;
            tblPanel.RowStyles.Clear();
            tblPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10.0f));
            tblPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90.0f));

            var tblListAndDescription = new TableLayoutPanel();
            tblListAndDescription.ColumnStyles.Clear();
            tblListAndDescription.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.0f));
            tblListAndDescription.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.0f));
            tblListAndDescription.Margin = new Padding(0);

            itemList = new PagedListUserControl<Item>();
            itemList.Padding = new Padding(PaddingValue);
            lblDescription = this.DockFillLabel("lblDescription", Color.White);
            lblDescription.Margin = new Padding(PaddingValue);

            tblListAndDescription.Controls.Add(itemList, 0, 0);
            tblListAndDescription.Controls.Add(lblDescription, 1, 0);

            lblTitle = this.DockFillLabel("lblTitle", Color.Red);
            lblTitle.Margin = new Padding(PaddingValue);
            tblPanel.Controls.Add(lblTitle, 0, 0);
            tblPanel.Controls.Add(tblListAndDescription, 0, 1);

            this.Controls.Add(tblPanel);

            tblPanel.Dock = DockStyle.Fill;
            tblListAndDescription.Dock = DockStyle.Fill;
            itemList.Dock = DockStyle.Fill;

            itemList.Stringify = (item) => item.Name;
            itemList.KeyUp += ItemList_KeyUp;

            lblTitle.Text = "__--== INVENTORY ==--__";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            initialWidth = Width;
            initialHeight = Height;
            initialFontSize = lblTitle.Font.Size;
        }

        private void ItemList_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        public void Notify(ControllerCommand cmd)
        {
            if(controlledBackpack != null)
            {
                switch(cmd)
                {
                    case ControllerCommand.Backpack_SelectNext:
                        itemList.SelectNext();
                        updateDescription();
                        break;
                    case ControllerCommand.Backpack_SelectPrevious:
                        itemList.SelectPrevious();
                        updateDescription();
                        break;
                    case ControllerCommand.Backpack_Close:
                        ValidIndex = false;
                        this.Hide();
                        gamePanel.Show();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Backpack_Pick:
                        ValidIndex = true;
                        this.Hide();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Backpack_Open:
                        Opened = true;
                        gamePanel.Hide();
                        this.Show();
                        itemList.Items = controlledBackpack.ToArray();
                        this.Refresh();
                        this.Focus();
                        break;
                }
            }

            itemList.Refresh();
        }

        private void updateDescription()
        {
            var ix = itemList.SelectedIndex;
            if (ix != -1)
            {
                lblDescription.Text = controlledBackpack[ix].FullDescription;
            }
            else
            {
                lblDescription.Text = "";
            }
        }

        private void FocusOnMap()
        {
            gamePanel.Show();
            FocusOnMap(gamePanel.Controls);
        }

        private void FocusOnMap(System.Windows.Forms.Control.ControlCollection collection)
        {
            foreach (System.Windows.Forms.Control ctrl in collection)
            {
                var type = ctrl.GetType();

                if (type == typeof(MapUserControl))
                {
                    ctrl.Focus();
                }
                else if(ctrl.HasChildren)
                {
                    FocusOnMap(ctrl.Controls);
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W:
                    Notify(ControllerCommand.Backpack_SelectPrevious);
                    break;
                case Keys.A:
                    // TODO: Previous page
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Backpack_SelectNext);
                    break;
                case Keys.D:
                    // TODO: Next page
                    break;
                case Keys.Enter:
                    Notify(ControllerCommand.Backpack_Pick);
                    break;
                case Keys.Escape:
                    Notify(ControllerCommand.Backpack_Close);
                    break;
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            updateDescription();
            base.OnEnter(e);
        }

        public void Register(Backpack backpack)
        {
            controlledBackpack = backpack;
            //itemList.Items = backpack.ToArray();
            //updateDescription();
        }

        public void Unregister(Backpack backpack)
        {
            controlledBackpack = null;
            itemList.Items = null;
        }

        public void NotifyAdd(Item[] itemsInBackpack)
        {
            itemList.Items = itemsInBackpack;
            updateDescription();
        }

        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();
            // Get the proportionality of the resize
            float proportionalNewWidth = (float)Width / initialWidth;
            float proportionalNewHeight = (float)Height / initialHeight;

            // Calculate the current font size
            lblTitle.Font = new Font(lblTitle.Font.FontFamily,
                                initialFontSize *
                                (proportionalNewWidth > proportionalNewHeight
                                ? proportionalNewHeight
                                : proportionalNewWidth),
                                lblTitle.Font.Style);
            ResumeLayout();

            base.OnResize(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }
    }
}
