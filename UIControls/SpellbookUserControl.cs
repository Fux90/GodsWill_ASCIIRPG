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
using GodsWill_ASCIIRPG.View;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class SpellbookUserControl : UserControl, SpeelbookController, ISpellbookViewer
    {
        private const int PaddingValue = 4;

        Spellbook controlledSpellbook;

        TableLayoutPanel gamePanel;
        DescriptionList<Spell> descriptionList;

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

        public SpellbookUserControl(TableLayoutPanel gamePanel)
        {
            InitializeComponent();

            this.gamePanel = gamePanel;
            this.BackColor = Color.Black;

            descriptionList = new DescriptionList<Spell>();
            descriptionList.KeyUp += OnKeyUp;
            descriptionList.Dock = DockStyle.Fill;
            descriptionList.Stringify = (item) => String.Format("[{0}] {1}",
                                                                descriptionList.Items.IndexOf(item),
                                                                item.Name);
            this.Controls.Add(descriptionList);

            descriptionList.Title = "__--== SPELL BOOK ==--__";
        }

        public void Notify(ControllerCommand cmd)
        {
            if (controlledSpellbook != null)
            {
                switch (cmd)
                {
                    case ControllerCommand.Spellbook_SelectNext:
                        descriptionList.SelectNext();
                        break;
                    case ControllerCommand.Spellbook_SelectPrevious:
                        descriptionList.SelectPrevious();
                        break;
                    case ControllerCommand.Spellbook_SelectNextPage:
                        descriptionList.SelectNextPage();
                        break;
                    case ControllerCommand.Spellbook_SelectPreviousPage:
                        descriptionList.SelectPreviousPage();
                        break;
                    case ControllerCommand.Spellbook_Close:
                        ValidIndex = false;
                        this.Hide();
                        gamePanel.Show();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Spellbook_Pick:
                        ValidIndex = true;
                        this.Hide();
                        FocusOnMap();
                        Opened = false;
                        break;
                    case ControllerCommand.Spellbook_Open:
                        Opened = true;
                        gamePanel.Hide();
                        this.Show();
                        descriptionList.Items = controlledSpellbook.ToList();
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
                else if (ctrl.HasChildren)
                {
                    FocusOnMap(ctrl.Controls);
                }
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    Notify(ControllerCommand.Spellbook_SelectPrevious);
                    break;
                case Keys.A:
                    Notify(ControllerCommand.Spellbook_SelectPreviousPage);
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Spellbook_SelectNext);
                    break;
                case Keys.D:
                    Notify(ControllerCommand.Spellbook_SelectNextPage);
                    break;
                case Keys.Enter:
                    Notify(ControllerCommand.Spellbook_Pick);
                    break;
                case Keys.Escape:
                    Notify(ControllerCommand.Spellbook_Close);
                    break;
            }
        }

        public void Register(Spellbook spellbook)
        {
            controlledSpellbook = spellbook;
        }

        public void Unregister(Spellbook spellbook)
        {
            controlledSpellbook = null;
            //itemList.Items = null;
            descriptionList.Items = null;
        }

        public void NotifyAdd(Spell[] spellsInSpellbook)
        {
            descriptionList.Items = spellsInSpellbook.ToList();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }
    }
}
