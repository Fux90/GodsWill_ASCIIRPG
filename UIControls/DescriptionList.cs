using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.UIControls;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class DescriptionList : UserControl
    {
        private const int PaddingValue = 4;

        PagedListUserControl<Item> itemList;
        Label lblDescription;
        Label lblTitle;
        Label lblPageNumber;

        private int initialWidth;
        private int initialHeight;
        private float initialFontSize;

        public int SelectedIndex
        {
            get
            {
                var ix = itemList.SelectedIndex;
                //ValidIndex = false;
                return ix;
            }
        }

        public string Title
        {
            get
            {
                return lblTitle.Text;
            }

            set
            {
                lblTitle.Text = value;
                UpdateDescription();
            }
        }

        public Item[] Items
        {
            get
            {
                return itemList.Items;
            }

            set
            {
                itemList.Items = value;
                UpdateDescription();
            }
        }

        public PagedListUserControl<Item>.StringifyMethod Stringify
        {
            get { return itemList.Stringify; }
            set { itemList.Stringify = value; }
        }

        public DescriptionList()
        {
            InitializeComponent();

            this.BackColor = Color.Black;

            var tblPanel = new TableLayoutPanel();

            tblPanel.Dock = DockStyle.Fill;

            tblPanel.BackColor = Color.LightGray;
            tblPanel.RowStyles.Clear();
            tblPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10.0f)); // Title
            tblPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90.0f));

            var tblListAndDescription = new TableLayoutPanel();
            tblListAndDescription.Dock = DockStyle.Fill;
            tblListAndDescription.ColumnStyles.Clear();
            tblListAndDescription.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.0f));
            tblListAndDescription.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.0f));
            tblListAndDescription.Margin = new Padding(0);

            var tblListAndPages = new TableLayoutPanel();
            tblListAndPages.Dock = DockStyle.Fill;
            tblListAndPages.RowStyles.Clear();
            tblListAndPages.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            tblListAndPages.RowStyles.Add(new RowStyle(SizeType.Absolute, 30.0f)); // Page Indication
            tblListAndPages.BackColor = Color.Orange;

            itemList = new PagedListUserControl<Item>();
            itemList.Padding = new Padding(PaddingValue);
            itemList.Dock = DockStyle.Fill;
            lblPageNumber = this.DockFillLabel("lblPageNumber", Color.White);
            lblPageNumber.TextAlign = ContentAlignment.MiddleCenter;
            lblPageNumber.Padding = new Padding(PaddingValue);

            tblListAndPages.Controls.Add(itemList, 0, 0);
            tblListAndPages.Controls.Add(lblPageNumber, 0, 1);

            lblDescription = this.DockFillLabel("lblDescription", Color.White);
            lblDescription.Margin = new Padding(PaddingValue);

            tblListAndDescription.Controls.Add(tblListAndPages, 0, 0);
            tblListAndDescription.Controls.Add(lblDescription, 1, 0);

            lblTitle = this.DockFillLabel("lblTitle", Color.Red);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Margin = new Padding(PaddingValue);
            tblPanel.Controls.Add(lblTitle, 0, 0);
            tblPanel.Controls.Add(tblListAndDescription, 0, 1);

            this.Controls.Add(tblPanel);

            //lblTitle.Text = "__--== INVENTORY ==--__";
            initialWidth = Width;
            initialHeight = Height;
            initialFontSize = lblTitle.Font.Size;
        }

        public new event KeyEventHandler KeyUp
        {
            add { itemList.KeyUp += value; }
            remove { itemList.KeyUp -= value; }
        }

        public void SelectPrevious()
        {
            itemList.SelectPrevious();
            UpdateDescription();
        }

        public void SelectNext()
        {
            itemList.SelectNext();
            UpdateDescription();
        }

        public void SelectPreviousPage()
        {
            itemList.SelectPreviousPage();
            UpdateDescription();
        }

        public void SelectNextPage()
        {
            itemList.SelectNextPage();
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            var ix = itemList.SelectedIndex;

            if (ix != -1)
            {
                lblDescription.Text = itemList.Items[ix].FullDescription;
                lblPageNumber.Text = String.Format("Page {0}/{1}",
                                                    itemList.SelectedPage + 1,
                                                    itemList.PagesCount);
            }
            else
            {
                lblDescription.Text = "";
                lblPageNumber.Text = "***";
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            UpdateDescription();
            base.OnEnter(e);
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
