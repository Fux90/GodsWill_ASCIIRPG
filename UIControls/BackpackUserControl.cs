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

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class BackpackUserControl : UserControl, BackpackController, IBackpackViewer
    {
        Backpack controlledBackpack;

        public BackpackUserControl()
        {
            InitializeComponent();
        }

        public void Notify(ControllerCommand cmd)
        {
            if(controlledBackpack != null)
            {
                switch(cmd)
                {

                }
            }
        }

        public void Register(Backpack backpack)
        {
            controlledBackpack = backpack;
        }

        public void Unregister()
        {
            controlledBackpack = null;
        }
    }
}
