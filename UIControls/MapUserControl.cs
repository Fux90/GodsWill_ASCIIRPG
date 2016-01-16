//#define DRAW_ALL
//#define DRAW_NO_SCROLL
//#define DEBUG_LINE
#define DEBUG_CENTERING
#define DEBUG_CENTER_VIEWPORT

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
    public partial class MapUserControl : UserControl, PgController, AIController, IMapViewer
    {
        public enum Modes
        {
            Normal,
            Selection
        }
        
        const float charSize = 10.0f;

        private readonly SelectorCursor selectorCursor = new SelectorCursor(); 
        Pg controlledPg;
        Atom currentAtomFollowedByViewport;

        List<AICharacter> aiCharacters;
#if DEBUG_LINE
        Line line;
#endif
        BackpackController backpackController;
        //MapController mapController;
        GameForm gameForm;

        private int viewportWidthInCells
        {
            get { return (int)Math.Ceiling((float)this.Width / charSize); }
        }
        private int viewportHeightInCells
        {
            get { return (int)Math.Ceiling((float)this.Height / charSize); }
        }

        private float HalfWidth { get { return Width / 2.0f; } }
        private float HalfHeight { get { return Height / 2.0f; } }

        private int viewportHeightInCells_2 { get { return viewportHeightInCells / 2; } }
        private int viewportHeightInCells_4 { get { return viewportHeightInCells / 4; } }
        private int viewportWidthInCells_2 { get { return viewportWidthInCells / 2; } }
        private int viewportWidthInCells_4 { get { return viewportWidthInCells / 4; } }

        private Coord centerRegion;

        private int HorizontalMaxDistanceFromCenter
        {
            get
            {
                return viewportWidthInCells_4;
            }
        }

        private int VerticalMaxDistanceFromCenter
        {
            get
            {
                return viewportHeightInCells_4;
            }
        }

        private int RegionLeft
        {
            get { return (int)Math.Max(centerRegion.X - viewportWidthInCells_2, 0); }
        }
        private int RegionRight
        {
            get
            {
                return (int)Math.Min(   RegionLeft + viewportWidthInCells, 
                                        controlledPg.Map.Width - 1);
            }
        }
        private int RegionTop
        {
            get { return (int)Math.Max(centerRegion.Y - viewportHeightInCells_2, 0); }
        }
        private int RegionBottom
        {
            get
            {
                return (int)Math.Min(   RegionTop + viewportHeightInCells, 
                                        controlledPg.Map.Height - 1); }
        }

        private Modes mode;
        private void EnterSelectionMode()
        {
            //selectorCursor.CenterOnPg(controlledPg);
            selectorCursor.Show(controlledPg.Map, controlledPg, controlledPg.PerceptionRange);
            mode = Modes.Selection;
            this.Refresh();
        }

        private void ExitSelectionMode()
        {
            mode = Modes.Normal;
            selectorCursor.Hide();
            this.Refresh();
        }

        public MapUserControl(  GameForm gameForm, 
                                BackpackController backpackController,
                                IAtomListener selectorMsgListener)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, charSize);

            this.aiCharacters = new List<AICharacter>();
            this.backpackController = backpackController;
            this.gameForm = gameForm;

            this.selectorCursor.RegisterListener(selectorMsgListener);
        }

        public void CenterOnPlayer()
        {
            if(controlledPg != null)
            {
                centerRegion = new Coord(controlledPg.Position);
                currentAtomFollowedByViewport = controlledPg;
            }
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

                    #region DEITY
                    case ControllerCommand.Player_Pray:
                        controlledPg.Pray(out acted);
                        
                        break; 
                    #endregion

                    #region AIs
                    case ControllerCommand.AI_Turn:
                        aiCharacters.ForEach(aiChar => aiChar.AI.ExecuteAction());
                        break;
                    #endregion

                    #region MSG_CONSOLE_HANDLING
                    case ControllerCommand.Player_ScrollMsgsUp:
                        controlledPg.ScrollUpMessages();
                        break;
                    case ControllerCommand.Player_ScrollMsgsDown:
                        controlledPg.ScrollDownMessages();
                        break;
                    #endregion

                    #region SELECTION
                    case ControllerCommand.Player_EnterSelectionMode:
                        EnterSelectionMode();
                        break;
                    case ControllerCommand.Player_ExitSelectionModeWithoutSelection:
                        ExitSelectionMode();
                        break;
                    case ControllerCommand.SelectionCursor_MoveNorth:
                        selectorCursor.Move(Direction.North, out acted);
                        break;
                    case ControllerCommand.SelectionCursor_MoveEast:
                        selectorCursor.Move(Direction.East, out acted);
                        break;
                    case ControllerCommand.SelectionCursor_MoveSouth:
                        selectorCursor.Move(Direction.South, out acted);
                        break;
                    case ControllerCommand.SelectionCursor_MoveWest:
                        selectorCursor.Move(Direction.West, out acted);
                        break;
                    case ControllerCommand.SelectionCursor_PickedCell:
                        MessageBox.Show("Selected cell " + selectorCursor.Position.ToString());
                        ExitSelectionMode();
                        break;
                    #endregion

                    case ControllerCommand.Player_ExitGame:
                        gameForm.Close();
                        break;
                }

                if(acted)
                {
                    controlledPg.EffectOfTurn();
                    Notify(ControllerCommand.AI_Turn);
                    aiCharacters.RemoveAll(aiChar => aiChar.Dead);
                }
            }
        }

        public void Register(Pg pg)
        {
            controlledPg = pg;
            CenterOnPlayer();
        }

        public void Unregister(Pg pg)
        {
            controlledPg = null;
        }

        public void NotifyMovement(Atom movedAtom, Coord freedCell, Coord occupiedCell)
        {
            if(currentAtomFollowedByViewport != null
                && currentAtomFollowedByViewport == movedAtom) // It should always been set
            {
                var diff = occupiedCell - centerRegion;
                if(Math.Abs(diff.X) > HorizontalMaxDistanceFromCenter)
                {
                    centerRegion.X = diff.X > 0
                        ? Math.Min(controlledPg.Map.Width - viewportWidthInCells_2 + 1, centerRegion.X + 1)
                        : Math.Max(0, centerRegion.X - 1);
                }
                if (Math.Abs(diff.Y) > VerticalMaxDistanceFromCenter)
                {
                    centerRegion.Y = diff.Y > 0
                        ? Math.Min(controlledPg.Map.Height - viewportHeightInCells_2 + 1, centerRegion.Y + 1)
                        : Math.Max(0, centerRegion.Y - 1);
                }
            }
            this.Refresh();
        }

        public void NotifyRemoval(Coord freedCell)
        {
            this.Refresh();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (mode)
            {
                case Modes.Normal:
                {
                    switch (e.KeyCode)
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
                        #region DEITY
                        case Keys.K:
                            Notify(ControllerCommand.Player_Pray);
                            break;
                        #endregion

                        #region MSG_CONSOLE_HANDLING
                        case Keys.PageUp:
                            Notify(ControllerCommand.Player_ScrollMsgsUp);
                            break;
                        case Keys.PageDown:
                            Notify(ControllerCommand.Player_ScrollMsgsDown);
                            break;
                        #endregion

                        case Keys.Escape:
                            Notify(ControllerCommand.Player_ExitGame);
                            break;

                        case Keys.X:
                            Notify(ControllerCommand.Player_EnterSelectionMode);
                            break;
                    }
                }
                break;

                case Modes.Selection:
                {
                    switch (e.KeyCode)
                    {
                        #region CURSOR_MOVEMENT
                        case Keys.W:
                            Notify(ControllerCommand.SelectionCursor_MoveNorth);
                            break;
                        case Keys.A:
                            Notify(ControllerCommand.SelectionCursor_MoveWest);
                            break;
                        case Keys.S:
                            Notify(ControllerCommand.SelectionCursor_MoveSouth);
                            break;
                        case Keys.D:
                            Notify(ControllerCommand.SelectionCursor_MoveEast);
                            break;
                            #endregion

                        case Keys.Enter:
                            Notify(ControllerCommand.SelectionCursor_PickedCell);
                            break;
                        case Keys.Escape:
                            Notify(ControllerCommand.Player_ExitSelectionModeWithoutSelection);
                            break;
                    }    
                }
                break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var map = controlledPg.Map;

#if DRAW_ALL
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
#elif DRAW_NO_SCROLL
            #region NO_SCROLL
            var firstCol = RegionLeft;
            var lastCol = RegionRight;
            var firstRow = RegionTop;
            var lastRow = RegionBottom;

            var w = viewportWidthInCells;
            var h = viewportHeightInCells;

            var numRows = map.Height;
            var numCols = map.Width;

            var offSetY = numRows <= h 
                ? HalfHeight - (float)numRows / 2.0f * charSize 
                : 0.0f;
            var offSetX = numCols <= w 
                ? HalfWidth - (float)numCols / 2.0f * charSize
                : 0.0f;

            for (int r = firstRow; r <= lastRow; r++)
            {
                var coord = new Coord() { Y = r };
                var yPos = r * charSize + offSetY;

                for (int c = firstCol; c <= lastCol; c++)
                {
                    coord.X = c;
                    var xPos = c * charSize + offSetX;
                    var atom = map[coord];
                    g.DrawString(atom.Symbol.ToString(),
                                    this.Font,
                                    new SolidBrush(atom.Color),
                                    new PointF(xPos, yPos));
                }
            }
            #endregion
#else
            var firstCol = RegionLeft;
            var lastCol = RegionRight;
            var firstRow = RegionTop;
            var lastRow = RegionBottom;

            var w = viewportWidthInCells;
            var h = viewportHeightInCells;

            var numRows = map.Height;
            var numCols = map.Width;

            var offSetY = numRows <= h 
                ? HalfHeight - (float)numRows / 2.0f * charSize 
                : 0.0f;
            var offSetX = numCols <= w 
                ? HalfWidth - (float)numCols / 2.0f * charSize
                : 0.0f;

            int xCell = 0;
            int yCell = 0;

            for (int r = firstRow; r <= lastRow; r++, yCell++)
            {
                var coord = new Coord() { Y = r };
                var yPos = yCell * charSize + offSetY;
                xCell = 0;

                for (int c = firstCol; c <= lastCol; c++, xCell++)
                {
                    coord.X = c;
                    if (map.IsCellExplored(coord))
                    {
                        var xPos = xCell * charSize + offSetX;
                        var atom = map[coord];
                        g.DrawString(atom.Symbol,
                                        this.Font,
                                        new SolidBrush(atom.Color),
                                        new PointF(xPos, yPos));
                        var untangibles = (AtomCollection)map[coord, Map.LevelType.Untangibles];
                        untangibles.ForEach(uAtom => g.DrawString(uAtom.Symbol,
                                                        this.Font,
                                                        new SolidBrush(uAtom.Color),
                                                        new PointF(xPos, yPos)));
                    }
                }
            }
#endif
#if DEBUG_CENTER_VIEWPORT
            #region CENTER_VIEWPORT
            var ptCenter = new PointF(  (centerRegion.X - firstCol) * charSize + offSetX,
                                        (centerRegion.Y - firstRow) * charSize + offSetY);
            g.DrawString("*", this.Font, Brushes.Orange, ptCenter);
            #endregion
#endif
#if DEBUG_CENTERING
            #region CENTERING
            g.DrawLine(Pens.Blue, new Point(0, 0), new Point(Width, Height));
            g.DrawLine(Pens.Blue, new Point(0, Height), new Point(Width, 0));
            var r_mapW2 = map.Width % 2;
            var r_mapH2 = map.Width % 2;
            int[] posWs = null;
            int[] posHs = null;
            var w2 = (float)(map.Width - 1) / 2.0f;
            var h2 = (float)(map.Height - 1) / 2.0f;
            if (r_mapW2 == 0)
            {
                posWs = new int[]
                {
                    (int)Math.Floor(w2),
                    (int)Math.Ceiling(w2),
                };
            }
            else
            {
                posWs = new int[]
                {
                    (int)w2,
                };
            }
            if (r_mapH2 == 0)
            {
                posHs = new int[]
                {
                    (int)Math.Floor(h2),
                    (int)Math.Ceiling(h2),
                };
            }
            else
            {
                posHs = new int[]
                {
                    (int)h2,
                };
            }

            foreach (var posX in posWs)
            {
                foreach (var posY in posHs)
                {
                    var ptF = new PointF(   (posX - firstCol) * charSize + offSetX, 
                                            (posY - firstRow) * charSize + offSetY);
                    g.DrawString("*", this.Font, Brushes.Yellow, ptF);
                }
            }
            #endregion
#endif
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
