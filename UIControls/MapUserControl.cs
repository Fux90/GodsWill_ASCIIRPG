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
    public partial class MapUserControl : UserControl, PgController, IMapViewer
    {
        const float charSize = 10.0f;

        Pg controlledPg;
        BackpackController backpackController;
        //MapController mapController;
        GameForm gameForm;

        public MapUserControl(GameForm gameForm, BackpackController backpackController)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, charSize);
            this.backpackController = backpackController;
            this.gameForm = gameForm;
        }

        public BackpackController BackpackController
        {
            get
            {
                return backpackController;
            }
        }

        //public MapController MapController
        //{
        //    get
        //    {
        //        return mapController;
        //    }
        //}

        public void Notify(ControllerCommand cmd)
        {
            if (controlledPg != null)
            {
                var acted = false;

                switch (cmd)
                {
                    #region MOVEMENT
                    case ControllerCommand.Player_MoveNorth:
                        controlledPg.Move(Direction.North, out acted);
                        break;
                    case ControllerCommand.Player_MoveWest:
                        controlledPg.Move(Direction.West, out acted);
                        break;
                    case ControllerCommand.Player_MoveSouth:
                        controlledPg.Move(Direction.South, out acted);
                        break;
                    case ControllerCommand.Player_MoveEast:
                        controlledPg.Move(Direction.East, out acted);
                        break;
                    #endregion

                    #region OBJECT_MANAGEMENT
                    case ControllerCommand.Player_PickUp:
                        if(controlledPg.PickUp())
                        {
                            //backpackController.Register(controlledPg.Backpack);
                        }
                        break;
                    case ControllerCommand.Backpack_Open:
                        backpackController.Notify(ControllerCommand.Backpack_Open);
                        break;
                    case ControllerCommand.Player_HandleWeapon:
                        backpackController.Notify(ControllerCommand.Backpack_Open);
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (backpackController.ValidIndex)
                            {
                                controlledPg.HandleWeapon(controlledPg.Backpack[backpackController.SelectedIndex]);
                            }
                        });
                        break;
                    case ControllerCommand.Player_UnhandleWeapon:
                        controlledPg.UnhandleWeapon();
                        break;
                    #endregion

                    case ControllerCommand.Player_ExitGame:
                        gameForm.Close();
                        break;
                }

                if(acted)
                {
                    controlledPg.EffectOfTurn();
                    // TODO AIs controls
                }
            }
        }

        public void Register(Pg pg)
        {
            controlledPg = pg;
        }

        public void Unregister()
        {
            controlledPg = null;
        }

        public void NotifyMovement(Coord freedCell, Coord occupiedCell)
        {
            this.Refresh();
        }

        public void NotifyRemoval(Coord freedCell)
        {
            this.Refresh();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                #region MOVEMENT
                case Keys.W:
                    Notify(ControllerCommand.Player_MoveNorth);
                    break;
                case Keys.A:
                    Notify(ControllerCommand.Player_MoveWest);
                    break;
                case Keys.S:
                    Notify(ControllerCommand.Player_MoveSouth);
                    break;
                case Keys.D:
                    Notify(ControllerCommand.Player_MoveEast);
                    break;
                #endregion

                #region OBJECT_HANDLING
                case Keys.P:
                    Notify(ControllerCommand.Player_PickUp);
                    break;
                case Keys.I:
                    Notify(ControllerCommand.Backpack_Open);
                    break;
                case Keys.H:
                    Notify(e.Control 
                            ? ControllerCommand.Player_UnhandleWeapon
                            : ControllerCommand.Player_HandleWeapon);
                    break;
                #endregion

                case Keys.Escape:
                    Notify(ControllerCommand.Player_ExitGame);
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var map = controlledPg.Map;

            for (int r = 0; r < map.Height; r++)
            {
                var coord = new Coord() { Y = r };
                var yPos = r * charSize;

                for (int c = 0; c < map.Width; c++)
                {
                    coord.X = c;
                    var xPos = c * charSize;
                    var atom = map[coord];
                    g.DrawString(   atom.Symbol.ToString(), 
                                    this.Font, 
                                    new SolidBrush(atom.Color), 
                                    new PointF(xPos, yPos));  
                }
            }
        }
    }
}
