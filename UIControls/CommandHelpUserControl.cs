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
        readonly private Dictionary<Keys, Dictionary<Keys[], string>> commands =
            new Dictionary<Keys, Dictionary<Keys[], string>>()
            {

            };

        System.Windows.Forms.Control container;

        Brush titleBrush = new SolidBrush(Color.Red);
        Brush keyColor = new SolidBrush(Color.Orange);

        public CommandHelpUserControl(System.Windows.Forms.Control container)
        {
            InitializeComponent();
            this.container = container;

            container.Controls.Add(this);
            this.Dock = DockStyle.Fill;

            this.BackColor = Color.Black;

            Load += CommandHelpUserControl_Load;
            KeyUp += CommandHelpUserControl_KeyUp;
        }

        private void CommandHelpUserControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void CommandHelpUserControl_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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
    }
}
