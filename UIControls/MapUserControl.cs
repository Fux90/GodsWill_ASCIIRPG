//#define DRAW_ALL
//#define DRAW_NO_SCROLL
//#define DEBUG_LINE
//#define DEBUG_CIRCLE
#define DEBUG_CENTERING
#define DEBUG_CENTER_VIEWPORT
//#define PREVENT_AI
//#define DEBUG_ENEMY_SENSING
//#define DEBUG_SPELL_LAUNCH
#define DEBUG_PRINT_MAP // On print button, it saves a txt representation of the map
#define CHECK_SAME_Y_IN_PRITING
#define LOG_STAMPS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.View;
using GodsWill_ASCIIRPG.Model.Spells;
using System.Threading;
using GodsWill_ASCIIRPG.Main;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class MapUserControl 
        : UserControl, PgController, AIController, IMapViewer, IAnimationViewer, IPgViewer
    {
        #region CONST
        const bool squaredLook = false;
        const float charSize = 10.0f;
        const float charFontPadding = squaredLook ? 1.5f : .0f;
        const float charPaintHorPadding = squaredLook ? .0f : 2.0f;
        const string obscuredCell = "░";
        private readonly SelectorCursor selectorCursor = new SelectorCursor();

        const int _Alt = 100;
        const int _Shift = 10;
        const int _Ctrl = 1;

        #endregion

        public delegate bool AfterSelectionOperation(Coord selPos, bool allowOtherSelection = false);
        private readonly AfterSelectionOperation defaultAfterSelectionOp = (selPos, otherSel) => 
        {
            MessageBox.Show("Selected cell " + selPos.ToString());
            return false;
        };

        public enum Modes
        {
            Normal,
            Selection,
            AfterDeath
        }

        Pg controlledPg;
        Atom currentAtomFollowedByViewport;

        List<AICharacter> aiCharacters;
#if DEBUG_LINE
        Line line;
#endif
#if DEBUG_CIRCLE
        Circle circle;
#endif
        BackpackController backpackController;
        SpellbookController spellbookController;

        //MapController mapController;
        GameForm gameForm;
        TransparentPanel transparentPanel;

        private int viewportWidthInCells
        {
            get { return (int)Math.Ceiling((float)this.Width / charSize); }
        }
        private int viewportHeightInCells
        {
            get { return (int)Math.Ceiling((float)this.Height / /*charSize*/this.FontHeight); }
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
            get { return (int)Math.Max(centerRegion.X - viewportWidthInCells_2 - 1, 0); }
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
            get { return (int)Math.Max(centerRegion.Y - viewportHeightInCells_2 - 1, 0); }
        }
        private int RegionBottom
        {
            get
            {
                return (int)Math.Min(   RegionTop + viewportHeightInCells, 
                                        controlledPg.Map.Height - 1); }
        }

        private AfterSelectionOperation afterValidSelectionOperation;
        private AfterSelectionOperation CurrentAfterValidSelectionOperation
        {
            get
            {
                var op = afterValidSelectionOperation;
                afterValidSelectionOperation = null;
                return op == null ? defaultAfterSelectionOp : op;
            }

            set
            {
                afterValidSelectionOperation = value;
            }
        }

        private AfterSelectionOperation afterInalidSelectionOperation;
        private AfterSelectionOperation CurrentAfterInvalidSelectionOperation
        {
            get
            {
                var op = afterInalidSelectionOperation;
                afterInalidSelectionOperation = null;
                return op == null ? defaultAfterSelectionOp : op;
            }

            set
            {
                afterInalidSelectionOperation = value;
            }
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
                                SpellbookController spellbookController,
                                IAtomListener selectorMsgListener)
        {
            InitializeComponent();

            this.Size = new Size();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, charSize + charFontPadding);

            this.aiCharacters = new List<AICharacter>();
            this.backpackController = backpackController;
            this.spellbookController = spellbookController;
            this.gameForm = gameForm;

            this.MinimumSize = new Size((int)charSize, this.FontHeight);
            
            this.selectorCursor.RegisterListener(selectorMsgListener);

            TransparentPanelCreation();
        }

        private void TransparentPanelCreation()
        {
            transparentPanel = new TransparentPanel();

            this.Controls.Add(transparentPanel);
            //transparentPanel.Transparency = 100;

            transparentPanel.Paint += TransparentPanel_Paint;
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            transparentPanel.Size = gameForm.ClientSize;
        }

        private void TransparentPanel_Paint(object sender, PaintEventArgs e)
        {
            if (controlledPg != null)
            {
                if (controlledPg.Dead)
                {
                    var g = e.Graphics;

                    var deadStr = "YOU'RE DEAD";

                    var font = new Font(FontFamily.GenericMonospace,
                                        30.0f,
                                        FontStyle.Bold);

                    var len = g.MeasureString(deadStr, font);

                    var pos = new PointF((this.Width - len.Width) / 2.0f,
                                            (this.Height - len.Height) / 2.0f);
                    var posShadow = pos.Offset(0, 5.0f);

                    g.DrawString(deadStr,
                                 font,
                                 Brushes.Yellow,
                                 posShadow);

                    g.DrawString(deadStr,
                                 font,
                                 Brushes.Red,
                                 pos);
                }
            }
        }

        public void CenterOnPlayer()
        {
            if(controlledPg != null)
            {
                var diff = controlledPg.Position - centerRegion;
                
                centerRegion.X = Math.Max(  0,
                                            Math.Min(controlledPg.Map.Width - viewportWidthInCells_2 + 1,
                                                     controlledPg.Position.X + 1));
                centerRegion.Y = Math.Max(  0,
                                            Math.Min(controlledPg.Map.Height - viewportHeightInCells_2 + 1,
                                                     controlledPg.Position.Y + 1));
                
                //centerRegion = new Coord(controlledPg.Position);
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

        public SpellbookController SpellbookController
        {
            get
            {
                return spellbookController;
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
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (backpackController.ValidIndex)
                            {
                                var item = controlledPg.Backpack[backpackController.SelectedIndex];

                                switch (backpackController.RapidOperation)
                                {
                                    case RapidOperation.Embrace:
                                        controlledPg.EmbraceShield(item);
                                        break;
                                    case RapidOperation.Handle:
                                        controlledPg.HandleWeapon(item);
                                        break;
                                    case RapidOperation.PutOn:
                                        controlledPg.WearArmor(item);
                                        break;
                                    case RapidOperation.Use:
                                        controlledPg.UseItem(item);
                                        break;
                                    case RapidOperation.None:
                                    default:
                                        break;
                                }
                            }
                        });
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
                    case ControllerCommand.Player_ActivateWeaponPower:
                        controlledPg.ActivateSpecialAttack();
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
                    case ControllerCommand.Player_UseItem:
                        backpackController.Notify(ControllerCommand.Backpack_Open);
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (backpackController.ValidIndex)
                            {
                                controlledPg.UseItem(controlledPg.Backpack[backpackController.SelectedIndex]);
                            }
                        });
                        break;
                    #endregion

                    #region DEITY
                    case ControllerCommand.Player_Pray:
                        controlledPg.Pray(out acted);

                        break;
                    #endregion

                    #region AIs
                    case ControllerCommand.AI_Turn:
                        aiCharacters.ForEach(aiChar =>
                        {
                            if (aiChar.BlockedTurns > 0)
                            {
                                aiChar.SkipTurn();
                            }
                            else
                            {
                                aiChar.AI.ExecuteAction();
                            }
                        });
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
                        CurrentAfterValidSelectionOperation = defaultAfterSelectionOp;
                        CurrentAfterInvalidSelectionOperation(selectorCursor.Position);
                        CurrentAfterInvalidSelectionOperation = defaultAfterSelectionOp;
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
                        acted = CurrentAfterValidSelectionOperation(selectorCursor.Position);
                        ExitSelectionMode();
                        break;
                    case ControllerCommand.SelectionCursor_PickedCellOneOfMany:
                        ExitSelectionMode();
                        acted = CurrentAfterValidSelectionOperation(selectorCursor.Position, true);
                        break;
                    #endregion

                    #region SPELLS
                    case ControllerCommand.Player_CastSpell:
                        spellbookController.Notify(ControllerCommand.Spellbook_Open);
                        this.WaitForRefocusThenDo(() =>
                        {
                            if (spellbookController.ValidIndex)
                            {
                                var spellBuilder = (SpellBuilder)controlledPg.Spellbook[spellbookController.SelectedIndex];
                                var target = spellBuilder.Target;
                                var issues = false;

                                switch(target.TargetType)
                                {
                                    case TargetType.NumberOfTargets:
                                    {
                                        var maxEnemies = target.NumericParameter;
                                        var chosenEnemies = 0;
                                        var targets = new List<Atom>();
                                        targets.Clear();
                                        spellBuilder.NotifyListeners("Select target");

                                        AfterSelectionOperation op = null;
                                        op = (selPos, allowOtherSel) =>
                                        {
                                            var selTarget = controlledPg.Map[selPos];
                                            var spellType = spellBuilder.SpellToBuildType;
                                            var permittedType = typeof(Atom);

                                            if (typeof(HealSpell).IsAssignableFrom(spellType))
                                            {
                                            }
                                            else if (typeof(UtilitySpell).IsAssignableFrom(spellType))
                                            {
                                            }
                                            else if (typeof(AttackSpell).IsAssignableFrom(spellType))
                                            {
                                                permittedType = typeof(IDamageable);
                                            }

                                            // If target is of the expected family type
                                            if (permittedType.IsAssignableFrom(selTarget.GetType()))
                                            {
                                                if (!targets.Contains(selTarget))
                                                {
                                                    chosenEnemies++;
                                                    targets.Add(selTarget);
                                                }
                                                else
                                                {
                                                    spellBuilder.NotifyListeners("Target already selected");
                                                }

                                                if ((!allowOtherSel && targets.Count() > 0) || maxEnemies == chosenEnemies)
                                                {
                                                    spellBuilder.SetTargets(targets);
                                                    var spell = spellBuilder.Create(out issues);
                                                    if (!issues)
                                                    {
                                                        controlledPg.CastSpell(spell, out acted);
                                                        this.Refresh();
                                                        return acted;
                                                    }

                                                    return !issues;
                                                }
                                            }
                                            //else
                                            {
                                                spellBuilder.NotifyListeners("Select next target");
                                                // Select next target
                                                CurrentAfterValidSelectionOperation = (AfterSelectionOperation)op.Clone();
                                                EnterSelectionMode();
                                            }


                                            return acted;
                                        };
                                        CurrentAfterValidSelectionOperation = op;
                                        CurrentAfterInvalidSelectionOperation = (selPos, allowOtherSel) =>
                                        {
                                            acted = false;
                                            targets.Clear();
                                            spellBuilder.NotifyListeners("Spell dismissed");
                                            return true;
                                        };

                                        EnterSelectionMode();
                                    }
                                    break;
                                }
                            }
                        });
                        break;
                    #endregion

                    #region SAVE
                    case ControllerCommand.Player_SaveGame:
                        //controlledPg.Map.Save(@"currentLevel.map");
                        Game.Current.Save(@"current.game");
                        controlledPg.NotifyListeners("*GAME SAVED*");
                        break;
                    #endregion

                    case ControllerCommand.Player_BackToMainMenu:
                        Game.Current.InitialMenu(true);
                        break;
                    case ControllerCommand.Player_ExitGame:
                        gameForm.Close();
                        break;
                }

                if(acted)
                {
                    controlledPg.EffectOfTurn();
#if !PREVENT_AI
                    Notify(ControllerCommand.AI_Turn);
#endif
                    aiCharacters.RemoveAll(aiChar => aiChar.Dead);

                    if(controlledPg.BlockedTurns > 0)
                    {
                        controlledPg.SkipTurn();
                        Notify(ControllerCommand.AI_Turn);
                    }
                }
            }
            else // Some commands work without player too
            {
                switch (cmd)
                {
                    case ControllerCommand.Player_ExitGame:
                        gameForm.Close();
                        break;
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

        public void UnregisterAll()
        {
            controlledPg = null;
            aiCharacters.Clear();
            backpackController.UnregisterAll();
            spellbookController.UnregisterAll();
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

        public void NotifyExploration()
        {
            this.Refresh();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var modifiers = 0;
            if (e.Alt) modifiers += _Alt;
            if (e.Shift) modifiers += _Shift;
            if (e.Control) modifiers += _Ctrl;

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
                                switch(modifiers)
                                {
                                    case _Shift:
                                        Notify(ControllerCommand.Player_CastSpell);
                                        break;
                                    case _Ctrl:
                                        Notify(ControllerCommand.Player_SaveGame);
                                        break;
                                    default:
                                        Notify(ControllerCommand.Player_MoveSouth);
                                        break;
                                }
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
                            switch(modifiers)
                            {
                                case _Ctrl:
                                    Notify(ControllerCommand.Player_UnhandleWeapon);
                                    break;
                                case _Shift:
                                    Notify(ControllerCommand.Player_ActivateWeaponPower);
                                    break;
                                default:
                                    Notify(ControllerCommand.Player_HandleWeapon);
                                    break;
                            }
                            break;
                        case Keys.P:
                            switch (modifiers)
                            {
                                case _Ctrl:
                                    Notify(ControllerCommand.Player_PutOff);
                                    break;
                                case _Shift:
                                    //??? Notify(ControllerCommand.Player_ActivateArmorPower);
                                    break;
                                default:
                                    Notify(ControllerCommand.Player_PutOn);
                                    break;
                            }
                            break;
                        case Keys.B:
                                switch (modifiers)
                                {
                                    case _Ctrl:
                                        Notify(ControllerCommand.Player_PutAwayShield);
                                        break;
                                    case _Shift:
                                        // ?? Notify(ControllerCommand.Player_ActivateWeaponPower);
                                        break;
                                    default:
                                        Notify(ControllerCommand.Player_EmbraceShield);
                                        break;
                                }
                                break;
                            case Keys.U:
                                Notify(ControllerCommand.Player_UseItem);
                                break;
#endregion

#if DEBUG_LINE
                case Keys.L:
                    line = new Line(controlledPg.Position, aiCharacters[0].Position);
                    this.NotifyMovement(controlledPg, controlledPg.Position, controlledPg.Position);
                    break;
#endif
#if DEBUG_CIRCLE
                            case Keys.NumPad0:
                            circle = new FilledCircle(controlledPg.Position, 15);
                                this.NotifyMovement(controlledPg, controlledPg.Position, controlledPg.Position);
                                break;
#endif
#if DEBUG_SPELL_LAUNCH
                            case Keys.Add:
                                CurrentAfterSelectionOperation = (pos) =>
                                {
                                    var target = controlledPg.Map[pos];
                                    if (typeof(IDamageable).IsAssignableFrom(target.GetType()))
                                    {
                                        var spell = FireOrb.Create(controlledPg, ((IDamageable)target));
                                        spell.Launch();
                                    }
                                    else
                                    {
                                        controlledPg.NotifyListeners("Can'use in that target");
                                    }
                                    return false;
                                };
                                EnterSelectionMode();
                                break;
#endif
#if DEBUG_PRINT_MAP
                            case Keys.PrintScreen:
                                controlledPg.Map.SaveToTxt(@"currentMap.txt");
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
                        case Keys.Home:
                            Notify(ControllerCommand.Player_BackToMainMenu);
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
                        case Keys.Add:
                            Notify(ControllerCommand.SelectionCursor_PickedCellOneOfMany);
                            break;
                        case Keys.Escape:
                        Notify(ControllerCommand.Player_ExitSelectionModeWithoutSelection);
                        break;
                    }    
                }
                break;

                case Modes.AfterDeath:
                    switch(e.KeyCode)
                    {
                        case Keys.Enter:
                            Notify(ControllerCommand.Player_BackToMainMenu);
                            break;
                        default:
                            break;
                    }
                    
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
            CenterOnPlayer();

            if (transparentPanel != null)
            {
                transparentPanel.Size = this.ClientSize;
            }
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
                ? HalfHeight - (float)numRows / 2.0f * this.FontHeight//charSize 
                : 0.0f;
            var offSetX = numCols <= w 
                ? HalfWidth - (float)numCols / 2.0f * charSize
                : 0.0f;

            int xCell = 0;
            int yCell = 0;

            for (int r = firstRow; r <= lastRow; r++, yCell++)
            {
                var coord = new Coord() { Y = r };
                var yPos = yCell * this.FontHeight/*charSize*/ + offSetY;
                xCell = 0;
                var xPos = offSetX;

                for (int c = firstCol; c <= lastCol; c++, xCell++)
                {
                    coord.X = c;
                    //var xPos = xCell * charSize + offSetX;
                    
                    if (controlledPg.IsLightingCell(coord) && map.IsCellExplored(coord))
                    {
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
                    else
                    {
                        if (map.IsCellUnknown(coord))
                        {
#if DEBUG_ENEMY_SENSING
                            g.DrawString("@",
                                            this.Font,
                                            Brushes.Blue,
                                            new PointF(xPos, yPos));
#else
                            g.DrawString(Floor._Symbol,
                                            this.Font,
                                            new SolidBrush(Floor._Color),
                                            new PointF(xPos, yPos));
#endif
                        }
                        else
                        {

                            g.DrawString(obscuredCell,
                                            this.Font,
                                            Brushes.DimGray,
                                            new PointF(xPos, yPos));
                        }

                        var untangibles = (AtomCollection)map[coord, Map.LevelType.Untangibles];
                        untangibles.Where(  a => !a.HasToBeInStraightSight).ToList()
                                    .ForEach(uAtom =>   g.DrawString(uAtom.Symbol,
                                                        this.Font,
                                                        new SolidBrush(uAtom.Color),
                                                        new PointF(xPos, yPos)));
                    }

                    xPos += charSize - charPaintHorPadding;
                }
            }
#endif
#if DEBUG_CENTER_VIEWPORT
            #region CENTER_VIEWPORT
            
            var ptCenter = new PointF(  (centerRegion.X - firstCol) * charSize + offSetX - charPaintHorPadding,
                                        (centerRegion.Y - firstRow) * this.FontHeight + offSetY);
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
                    var ptF = new PointF(   (posX - firstCol) * charSize + offSetX - charPaintHorPadding, 
                                            (posY - firstRow) * /*charSize */this.FontHeight + offSetY);
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
                    var ptF = new PointF(pt.X * charSize + offSetX - charPaintHorPadding, pt.Y * charSize + offSetY);
                    g.DrawString("*", this.Font, Brushes.Red, ptF);
                }
            }
#endif
#if DEBUG_CIRCLE
            if (circle != null)
            {
                foreach (Coord pt in circle)
                {
                    var yPos = pt.Y * charSize + offSetY;
                    var xPos = pt.X * charSize + offSetX; - charPaintHorPadding
                    g.DrawString("*",
                                    this.Font,
                                    Brushes.Orange,
                                    new PointF(xPos, yPos));
                }
            }
#endif
            if (currentFrame != null)
            {
                currentFrame.ForEach(fI =>
                {
                    var pt = fI.Position;
                    var yPos = (pt.Y - firstRow) * /*charSize*/this.FontHeight + offSetY;
                    //var xPos = (pt.X - firstCol) * charSize + offSetX - charPaintHorPadding;
                    var xPos = (pt.X - firstCol) * (charSize - charPaintHorPadding) + offSetX;
                    
                    g.DrawString(   fI.Symbol,
                                    this.Font,
                                    new SolidBrush(fI.Color),
                                    new PointF(xPos, yPos));
                });
                currentFrame = null;
            }
        }

        public void Register(AICharacter character)
        {
            aiCharacters.AddOnce(character);
        }

        public void RegisterAll(IEnumerable<AICharacter> characters)
        {
            foreach (var character in characters)
            {
                aiCharacters.AddOnce(character);
            }
        }

        public void Unregister(AICharacter character)
        {
            aiCharacters.Remove(character);
        }

        Animation.Frame currentFrame;
        public void PlayFrame(Animation animation)
        {
            foreach (Animation.Frame frame in animation)
            {
                currentFrame = frame;
                this.Refresh();
                Thread.Sleep(Animation._InterFrameDelay);
            }
        }

        public void NotifyDeath()
        {
            mode = Modes.AfterDeath;
            transparentPanel.Transparency = 100;
            this.Refresh();
        }
    }
}
