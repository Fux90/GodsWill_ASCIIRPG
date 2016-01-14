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

        TableLayoutPanel gamePanel;
        DescriptionList descriptionList;

        public bool ValidIndex { get; private set; }

        public int SelectedIndex
        {
            get
            {
                ValidIndex = false;
                return descriptionList.SelectedIndex;
            }
        }

        public bool Opened { get; private set; }

        public BackpackUserControl(TableLayoutPanel gamePanel)
        {
            InitializeComponent();

            this.gamePanel = gamePanel;
            this.BackColor = Color.Black;

            descriptionList = new DescriptionList();
            descriptionList.KeyUp += OnKeyUp;
            descriptionList.Dock = DockStyle.Fill;
            descriptionList.Stringify = (item) => String.Format("[{0}] {1}",
                                                                descriptionList.Items.IndexOf(item),
                                                                item.Name);
            this.Controls.Add(descriptionList);

            descriptionList.Title = "__--== INVENTORY ==--__";
        }

        public void Notify(ControllerCommand cmd)
        {
            if(controlledBackpack != null)
            {
                switch(cmd)
                {
                    case ControllerCommand.Backpack_SelectNext:
                        descriptionList.SelectNext();
                        break;
                    case ControllerCommand.Backpack_SelectPrevious:
                        descriptionList.SelectPrevious();
                        break;
                    case ControllerCommand.Backpack_SelectNextPage:
                        descriptionList.SelectNextPage();
                        break;
                    case ControllerCommand.Backpack_SelectPreviousPage:
                        descriptionList.SelectPreviousPage();
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
                        descriptionList.Items = controlledBackpack.ToList();
                        this.Refresh();
                        this.Focus();
                        break;
                }
            }

            descriptionList.Refresh();
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

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W:
                    Notify(ControllerCommand.Backpack_SelectPrevious);
                    break;
                case Keys.A:
                    Notify(ControllerCommand.Backpack_SelectPreviousPage);
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Backpack_SelectNext);
                    break;
                case Keys.D:
                    Notify(ControllerCommand.Backpack_SelectNextPage);
                    break;
                case Keys.Enter:
                    Notify(ControllerCommand.Backpack_Pick);
                    break;
                case Keys.Escape:
                    Notify(ControllerCommand.Backpack_Close);
                    break;
            }
        }

        public void Register(Backpack backpack)
        {
            controlledBackpack = backpack;
        }

        public void Unregister(Backpack backpack)
        {
            controlledBackpack = null;
            //itemList.Items = null;
            descriptionList.Items = null;
        }

        public void NotifyAdd(Item[] itemsInBackpack)
        {
            descriptionList.Items = itemsInBackpack.ToList();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }
    }
}
