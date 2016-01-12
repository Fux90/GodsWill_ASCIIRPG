#define DEBUG_LINE

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
    public partial class MapUserControl : UserControl, PgController, AIController, IMapViewer
    {
        const float charSize = 10.0f;

        Pg controlledPg;
        List<AICharacter> aiCharacters;
#if DEBUG_LINE
        Line line;
#endif
        BackpackController backpackController;
        //MapController mapController;
        GameForm gameForm;

        public MapUserControl(GameForm gameForm, BackpackController backpackController)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, charSize);

            this.aiCharacters = new List<AICharacter>();
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
                        controlledPg.PickUp();
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
                    case ControllerCommand.Player_PutOn:
                        backpackController.Notify(ControllerCommand.Backpack_Open);
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (backpackController.ValidIndex)
                            {
                                controlledPg.WearArmor(controlledPg.Backpack[backpackController.SelectedIndex]);
                            }
                        });
                        break;
                    case ControllerCommand.Player_PutOff:
                        controlledPg.RemoveArmor();
                        break;
                    case ControllerCommand.Player_EmbraceShield:
                        backpackController.Notify(ControllerCommand.Backpack_Open);
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (backpackController.ValidIndex)
                            {
                                controlledPg.EmbraceShield(controlledPg.Backpack[backpackController.SelectedIndex]);
                            }
                        });
                        break;
                    case ControllerCommand.Player_PutAwayShield:
                        controlledPg.DisembraceShield();
                        break;
                    #endregion

                    #region AIs
                    case ControllerCommand.AI_Turn:
                        aiCharacters.ForEach(aiChar => aiChar.AI.ExecuteAction());
                        break;
                    #endregion

                    case ControllerCommand.Player_ExitGame:
                        gameForm.Close();
                        break;
                }

                if(acted)
                {
                    controlledPg.EffectOfTurn();

                    //Notify(ControllerCommand.AI_Turn);
                }
            }
        }

        public void Register(Pg pg)
        {
            controlledPg = pg;
        }

        public void Unregister(Pg pg)
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
                case Keys.G:
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
                case Keys.P:
                    Notify(e.Control
                            ? ControllerCommand.Player_PutOff
                            : ControllerCommand.Player_PutOn);
                    break;
                case Keys.B:
                    Notify(e.Control
                            ? ControllerCommand.Player_PutAwayShield
                            : ControllerCommand.Player_EmbraceShield);
                    break;
                #endregion

#if DEBUG_LINE
                case Keys.L:
                    line = new Line(controlledPg.Position, aiCharacters[0].Position);
                    this.NotifyMovement(controlledPg.Position, controlledPg.Position);
                    break;
#endif
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

#if DEBUG_LINE
            if (line != null)
            {
                foreach (Coord pt in line)
                {
                    var ptF = new PointF(pt.X * charSize, pt.Y * charSize);
                    g.DrawString("*", this.Font, Brushes.Red, ptF);
                }
            }
#endif
        }

        public void Register(AICharacter character)
        {
            aiCharacters.Add(character);
        }

        public void Unregister(AICharacter character)
        {
            aiCharacters.Remove(character);
        }
    }
}
