using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.View;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class MerchantUserControl : UserControl, MerchantController, IMerchantViewer
    {
        private const int PaddingValue = 4;
        const int _Alt = 100;
        const int _Shift = 10;
        const int _Ctrl = 1;

        Merchant controlledMerchant;
        Pg controlledPg;

        Backpack[] controlledBackpack;
        private Backpack[] ControlledBackpack
        {
            get
            {
                if(controlledBackpack == null)
                {
                    controlledBackpack = new Backpack[2];
                }

                return controlledBackpack;
            }
        }

        Label lblTitle;
        Label lblHelp;
        SingleMessageLogUserControl singleLogMerchant;

        TableLayoutPanel gamePanel;
        DescriptionList<Item>[] descriptionList;

        public bool ValidIndex { get; private set; }

        private int selectedListIndex;
        public int SelectedListIndex
        {
            get
            {
                return selectedListIndex;
            }

            private set
            {
                if(value < 0)
                {
                    selectedListIndex = (selectedListIndex + descriptionList.Length - 1) % descriptionList.Length;
                }
                else
                {
                    selectedListIndex = (selectedListIndex + 1) % descriptionList.Length;
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                ValidIndex = false;
                return descriptionList[SelectedListIndex].SelectedIndex;
            }
        }

        public bool Opened { get; private set; }

        public string Text
        {
            get
            {
                return lblTitle.Text;
            }

            set
            {
                lblTitle.Text = value == null ? "" : value;
            }
        }

        public string HelpString
        {
            get
            {
                return lblHelp.Text;
            }

            set
            {
                lblHelp.Text = value == null ? "" : value;
            }
        }

        public MerchantUserControl(TableLayoutPanel gamePanel)
        {
            InitializeComponent();

            this.gamePanel = gamePanel;
            this.BackColor = Color.Black;

            var structure = new TableLayoutPanel();

            structure.RowStyles.Clear();
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 10.0f)); // Title
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 80.0f));
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // Help
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // Merchant notifier

            lblTitle = this.DockFillLabel("lblTitle", Color.Red);
            lblTitle.Margin = new Padding(PaddingValue);
            
            descriptionList = new DescriptionList<Item>[2];
            for (int i = 0; i < descriptionList.Length; i++)
            {
                descriptionList[i] = new DescriptionList<Item>();
                descriptionList[i].KeyUp += OnKeyUp;
                descriptionList[i].Dock = DockStyle.Fill;
                descriptionList[i].Stringify = (item) => String.Format("[{0}] {1}",
                                                                    descriptionList[i].Items.IndexOf(item),
                                                                    item.Name);

                descriptionList[i].Title = i == 0 
                                    ? "You"
                                    : "Merchant";
                descriptionList[i].HelpString = "W: Previous - S: Next - A: Previous Page - D: Next Page - U: Use - H: Handle Weapon - P: Put on Armor - B: Embrace Shield";
            }

            var descriptionListPanel = new TableLayoutPanel();
            descriptionListPanel.ColumnStyles.Clear();
            descriptionListPanel.ColumnStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            descriptionListPanel.ColumnStyles.Add(new RowStyle(SizeType.Percent, 50.0f));

            lblHelp = this.DockFillLabel("lblHelp", Color.Yellow);
            lblHelp.Margin = new Padding(PaddingValue);

            singleLogMerchant = new SingleMessageLogUserControl();
            singleLogMerchant.Dock = DockStyle.Fill;

            descriptionListPanel.Controls.Add(descriptionList[0], 0, 0);
            descriptionListPanel.Controls.Add(descriptionList[1], 1, 0);

            structure.Controls.Add(lblTitle, 0, 0);
            structure.Controls.Add(descriptionListPanel, 0, 1);
            structure.Controls.Add(lblHelp, 0, 2);
            structure.Controls.Add(singleLogMerchant, 0, 3);

            this.Controls.Add(structure);
        }

        public void Notify(ControllerCommand cmd)
        {
            if (controlledBackpack[selectedListIndex] != null)
            {
                switch (cmd)
                {
                    case ControllerCommand.Merchant_SelectNext:
                        descriptionList[selectedListIndex].SelectNext();
                        break;
                    case ControllerCommand.Merchant_SelectPrevious:
                        descriptionList[selectedListIndex].SelectPrevious();
                        break;
                    case ControllerCommand.Merchant_SelectNextPage:
                        descriptionList[selectedListIndex].SelectNextPage();
                        break;
                    case ControllerCommand.Merchant_SelectPreviousPage:
                        descriptionList[selectedListIndex].SelectPreviousPage();
                        break;
                    case ControllerCommand.Merchant_SelectPreviousList:
                        descriptionList[selectedListIndex].HideSelection();
                        SelectedListIndex = SelectedListIndex - 1;
                        descriptionList[selectedListIndex].ShowSelection();
                        break;
                    case ControllerCommand.Merchant_SelectNextList:
                        descriptionList[selectedListIndex].HideSelection();
                        SelectedListIndex = SelectedListIndex + 1;
                        descriptionList[selectedListIndex].ShowSelection();
                        break;
                    case ControllerCommand.Merchant_Close:
                        ValidIndex = false;
                        this.Hide();
                        gamePanel.Show();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Merchant_PurchaseSell:
                        ValidIndex = true;
                        this.Hide();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Merchant_Open:
                        Opened = true;
                        gamePanel.Hide();
                        this.Show();
                        for (int i = 0; i < controlledBackpack.Length; i++)
                        {
                            descriptionList[i].Items = controlledBackpack[i].ToList();
                        }
                        this.Refresh();
                        this.Focus();
                        break;
                }
            }

            descriptionList[selectedListIndex].Refresh();
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
                else if (ctrl.HasChildren)
                {
                    FocusOnMap(ctrl.Controls);
                }
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            var modifiers = 0;
            if (e.Alt) modifiers += _Alt;
            if (e.Shift) modifiers += _Shift;
            if (e.Control) modifiers += _Ctrl;

            switch (e.KeyCode)
            {
                case Keys.W:
                    Notify(ControllerCommand.Merchant_SelectPrevious);
                    break;
                case Keys.A:
                    Notify(ControllerCommand.Merchant_SelectPreviousPage);
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Merchant_SelectNext);
                    break;
                case Keys.D:
                    Notify(ControllerCommand.Merchant_SelectNextPage);
                    break;
                case Keys.Tab:
                    if (modifiers == _Shift)
                    {
                        Notify(ControllerCommand.Merchant_SelectPreviousList);
                    }
                    else
                    {
                        Notify(ControllerCommand.Merchant_SelectNextList);
                    }
                    break;
                case Keys.Escape:
                    Notify(ControllerCommand.Merchant_Close);
                    break;
                case Keys.Enter:
                    Notify(ControllerCommand.Merchant_PurchaseSell);
                    break;
            }
        }

        public void UnregisterAll()
        {
            controlledPg = null;
            controlledMerchant = null;

            for (int i = 0; i < controlledBackpack.Length; i++)
            {
                controlledBackpack[i] = null;
                descriptionList[i].Items.Clear();
            }
        }

        public void NotifyAddRemoval(Item[] itemsInBackpack)
        {
            descriptionList[selectedListIndex].Items = itemsInBackpack.ToList();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        public void Register(Merchant merchant)
        {
            controlledMerchant = merchant;
            ControlledBackpack[1] = merchant.Backpack;
        }

        public void Unregister(Merchant merchant)
        {
            if (controlledMerchant == merchant)
            {
                controlledMerchant = null;
                ControlledBackpack[1] = null;
            }
        }

        public void Register(Pg pg)
        {
            controlledPg = pg;
            ControlledBackpack[0] = pg.Backpack;
        }

        public void Unregister(Pg pg)
        {
            if (controlledPg == pg)
            {
                controlledPg = null;
                ControlledBackpack[0] = null;
            }
        }

        public void BringUpAndFocus()
        {
            Notify(ControllerCommand.Backpack_Open);
        }
        
    }
}
