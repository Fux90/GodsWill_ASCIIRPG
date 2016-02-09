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

        private const float penWidth = 4.0f;
        private const float charSize = 10.0f;
        private const float charHelpSize = charSize / 2.5f;

        private readonly string[] helpStrings = new string[]
        {
            "Tab: Switch List - Enter: Sell - Esc: Exit",
            "Tab: Switch List - Enter: Buy - Esc: Exit",
        };

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
        TableLayoutPanel structure;
        TransparentPanel transparentPanel;

        DescriptionList<Item>[] descriptionList;

        public bool ValidIndex { get; private set; }

        private int selectedListIndex;
        private int initialWidth;
        private int initialHeight;
        private float initialTitleFontSize;
        private float initialHelpFontSize;

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
                    selectedListIndex = (value + descriptionList.Length) % descriptionList.Length;
                }
                else
                {
                    selectedListIndex = (value) % descriptionList.Length;
                }

                OnSelectedIndexChanged(new IndexChangedEventArgs(selectedListIndex));
            }
        }

        private class IndexChangedEventArgs : EventArgs
        {
            public int Index { get; private set; }

            public IndexChangedEventArgs(int index)
            {
                Index = index;
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if(SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, e);
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

            structure = new TableLayoutPanel();
            
            structure.RowStyles.Clear();
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 10.0f)); // Title
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 80.0f));
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // Help
            structure.RowStyles.Add(new RowStyle(SizeType.Percent, 5.0f)); // Merchant notifier

            lblTitle = this.DockFillLabel("lblTitle", Color.Red);
            lblTitle.Margin = new Padding(PaddingValue);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            descriptionList = new DescriptionList<Item>[2];
            for (int i = 0; i < descriptionList.Length; i++)
            {
                descriptionList[i] = new DescriptionList<Item>();
                descriptionList[i].KeyUp += OnKeyUp;
                descriptionList[i].Dock = DockStyle.Fill;
                var locI = i;
                descriptionList[i].Stringify = (item) => String.Format("[{0}] {1}",
                                                                    descriptionList[locI].Items.IndexOf(item),
                                                                    item.Name);

                descriptionList[i].Title = i == 0 
                                    ? "Your backpack"
                                    : "Merchant's warehouse";
                descriptionList[i].HelpString = "W: Previous - S: Next - A: Previous Page - D: Next Page";
            }

            var descriptionListPanel = new TableLayoutPanel();
            descriptionListPanel.ColumnStyles.Clear();
            descriptionListPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            descriptionListPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            descriptionListPanel.Margin = new Padding(PaddingValue);
            descriptionListPanel.Dock = DockStyle.Fill;

            lblHelp = this.DockFillLabel("lblHelp", Color.Yellow, new Font(FontFamily.GenericMonospace, charHelpSize));
            lblHelp.Margin = new Padding(PaddingValue);
            lblHelp.TextAlign = ContentAlignment.MiddleCenter;

            singleLogMerchant = new SingleMessageLogUserControl();
            singleLogMerchant.Dock = DockStyle.Fill;

            descriptionListPanel.Controls.Add(descriptionList[0], 0, 0);
            descriptionListPanel.Controls.Add(descriptionList[1], 1, 0);

            transparentPanel = new TransparentPanel();
            transparentPanel.BackColor = Color.Transparent;
            //transparentPanel.Transparency = 128;
            transparentPanel.Paint += TransparentPanel_Paint; ;
            singleLogMerchant.KeyUp += OnKeyUp;

            structure.Controls.Add(lblTitle, 0, 0);
            structure.Controls.Add(descriptionListPanel, 0, 1);
            structure.Controls.Add(lblHelp, 0, 2);
            structure.Controls.Add(singleLogMerchant, 0, 3);

            this.Controls.Add(structure);
            this.Controls.Add(transparentPanel);

            HelpString = helpStrings[SelectedListIndex];
            this.SelectedIndexChanged += (sender, e) =>
            {
                HelpString = helpStrings[((IndexChangedEventArgs)e).Index];
            };

            initialWidth = Width;
            initialHeight = Height;
            initialTitleFontSize = lblTitle.Font.Size;
            initialHelpFontSize = lblHelp.Font.Size;

            structure.Parent.Resize += (sender, e) => Controll_FillParent(sender, e, structure);
            transparentPanel.Parent.Resize += (sender, e) => Controll_FillParent(sender, e, transparentPanel);

            transparentPanel.BringToFront();
        }

        private void Controll_FillParent(object sender, EventArgs e, System.Windows.Forms.Control ctrl)
        {
            var parent = (System.Windows.Forms.Control)sender;

            ctrl.Size = parent.ClientSize;
        }

        private void TransparentPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var ctrl = (System.Windows.Forms.Control)descriptionList[SelectedListIndex];
            var pos = ctrl.Location;
            pos.Offset(Margin.Left, lblTitle.Height + 2 * (Margin.Top + Margin.Bottom));
            var size = ctrl.Size;
            size.Width += Padding.Left + Padding.Right;
            size.Height += Padding.Top + Padding.Bottom;

            g.DrawRectangle(new Pen(Brushes.Orange, penWidth),
                            new Rectangle(pos, size));
        }

        public void Notify(ControllerCommand cmd)
        {
            var tryedExchange = false;

            if (ControlledBackpack[SelectedListIndex] != null)
            {                
                switch (cmd)
                {
                    case ControllerCommand.Merchant_SelectNext:
                        descriptionList[SelectedListIndex].SelectNext();
                        break;
                    case ControllerCommand.Merchant_SelectPrevious:
                        descriptionList[SelectedListIndex].SelectPrevious();
                        break;
                    case ControllerCommand.Merchant_SelectNextPage:
                        descriptionList[SelectedListIndex].SelectNextPage();
                        break;
                    case ControllerCommand.Merchant_SelectPreviousPage:
                        descriptionList[SelectedListIndex].SelectPreviousPage();
                        break;
                    case ControllerCommand.Merchant_SelectPreviousList:
                        descriptionList[SelectedListIndex].HideSelection();
                        SelectedListIndex = SelectedListIndex - 1;
                        descriptionList[SelectedListIndex].ShowSelection();
                        this.Refresh();
                        break;
                    case ControllerCommand.Merchant_SelectNextList:
                        descriptionList[SelectedListIndex].HideSelection();
                        SelectedListIndex = SelectedListIndex + 1;
                        descriptionList[SelectedListIndex].ShowSelection();
                        this.Refresh();
                        break;
                    case ControllerCommand.Merchant_Close:
                        ValidIndex = false;
                        this.Hide();
                        gamePanel.Show();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Merchant_PurchaseSell:
                        if (SelectedIndex != -1)
                        {
                            var item = ControlledBackpack[SelectedListIndex][SelectedIndex];
                            var locIx = SelectedIndex;

                            if (SelectedListIndex == 0) // Merchant buy from Pg
                            {
                                if (controlledMerchant.Purchase(item, controlledPg))
                                {
                                    controlledMerchant.GoodPurchaseSpeech();
                                }
                                else
                                {
                                    controlledMerchant.BadPurchaseSpeech();
                                }
                            }
                            else // Merchant sell to Pg
                            {
                                if (controlledMerchant.Sell(item, controlledPg))
                                {
                                    Notify(ControllerCommand.Merchant_SelectPrevious);
                                    controlledMerchant.GoodSellSpeech();
                                }
                                else
                                {
                                    controlledMerchant.BadSellSpeech();
                                }
                            }

                            tryedExchange = true;
                        }
                        break;
                    case ControllerCommand.Merchant_Open:
                        Opened = true;
                        gamePanel.Hide();
                        SelectedListIndex = 0;
                        this.Show();
                        for (int i = 0; i < controlledBackpack.Length; i++)
                        {
                            if(i == 0)
                            {
                                descriptionList[i].ShowSelection();
                            }
                            else
                            {
                                descriptionList[i].HideSelection();
                            }
                            descriptionList[i].Items = controlledBackpack[i].ToList();
                        }
                        this.Refresh();
                        this.Focus();
                        break;
                }
            }

            if (!tryedExchange)
            {
                var ix = descriptionList[SelectedListIndex].SelectedIndex;
                if (ix != -1)
                {
                    if (SelectedListIndex == 0)
                    {
                        controlledMerchant.ProposePurchase(controlledBackpack[SelectedListIndex][ix]);
                    }
                    else
                    {
                        controlledMerchant.ProposeSell(controlledBackpack[SelectedListIndex][ix]);
                    }
                }
                else
                {
                    controlledMerchant.NoProposal();
                }

                
            }

            tryedExchange = false;
            descriptionList[SelectedListIndex].Refresh();
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
            if(controlledMerchant != null)
            {
                controlledMerchant.UnregisterListener(singleLogMerchant);
            }

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
            descriptionList[SelectedListIndex].Items = itemsInBackpack.ToList();
        }

        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();
            // Get the proportionality of the resize
            float proportionalNewWidth = (float)Width / initialWidth;
            float proportionalNewHeight = (float)Height / initialHeight;

            if (proportionalNewWidth > 0 && proportionalNewHeight > 0)
            {
                // Calculate the current font size
                lblTitle.Font = new Font(lblTitle.Font.FontFamily,
                                    initialTitleFontSize *
                                    (proportionalNewWidth > proportionalNewHeight
                                    ? proportionalNewHeight
                                    : proportionalNewWidth),
                                    lblTitle.Font.Style);

                lblHelp.Font = new Font(lblHelp.Font.FontFamily,
                                    initialHelpFontSize *
                                    (proportionalNewWidth > proportionalNewHeight
                                    ? proportionalNewHeight
                                    : proportionalNewWidth),
                                    lblHelp.Font.Style);
            }

            ResumeLayout();

            base.OnResize(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        public void Register(Merchant merchant)
        {
            controlledMerchant = merchant;
            ControlledBackpack[1] = merchant.Backpack;
            controlledMerchant.RegisterListener(singleLogMerchant);
        }

        public void Unregister(Merchant merchant)
        {
            if (controlledMerchant == merchant)
            {
                controlledMerchant.UnregisterListener(singleLogMerchant);
                controlledMerchant = null;
                ControlledBackpack[1] = null;
            }
        }

        public void Register(Pg pg)
        {
            controlledPg = pg;
            ControlledBackpack[0] = pg.Backpack;
            NotifyBuyerGold(pg.MyGold);
        }

        public void Unregister(Pg pg)
        {
            if (controlledPg == pg)
            {
                controlledPg = null;
                ControlledBackpack[0] = null;
            }
        }

        public void BringUpAndFocus(Pg interactor)
        {
            Register(interactor);
            Notify(ControllerCommand.Merchant_Open);
        }

        public void NotifyMerchantName(string merchantName)
        {
            Text = merchantName;
        }

        public void NotifyExcange()
        {
            for (int i = 0; i < ControlledBackpack.Length; i++)
            {
                descriptionList[i].Items = controlledBackpack[i].ToList();
            }
            this.Refresh();
        }

        public void NotifyBuyerGold(int gold)
        {
            descriptionList[0].Title = String.Format("Your Backpack - {0} $", gold);
        }
    }
}
