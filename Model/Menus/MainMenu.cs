using GodsWill_ASCIIRPG.Main;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG.Model.Menus
{
    public class MainMenu : Menu
    {
        Game currGame;

        public MainMenu(Game currGame, bool resumeActive = false)
            : base()
        {
            this.currGame = currGame;
            this.Title = "__--== MAIN MENU ==--__";
            CreateNewGameEntry();
            CreteLoadGameEntry();
            CreateDebugGameEntry();
            CreateResumeEntry(resumeActive);
            CreateExitEntry();
        }

        private void CreateResumeEntry(bool active)
        {
            MenuItem.MenuAction resume = (pars) => {
                Close();
            };

            this.AddMenuItem(new MenuItem(  "Resume",
                                            resume,
                                            active));
        }

        private void CreateExitEntry()
        {
            MenuItem.MenuAction exit = (pars) => {
                currGame.Exit();
            };

            this.AddMenuItem(new MenuItem("Exit Game",
                                            exit,
                                            true));
        }

        private void CreateDebugGameEntry()
        {
            MenuItem.MenuAction debugMap = (pars) => {
                currGame.ResetMapBuilder();
                currGame.CleanMsgs();
                currGame.DebugMap();
                ActivateByName("Resume");
                currGame.WorldReset();
                currGame.GameInitialization();
                Close();
            };

            this.AddMenuItem(new MenuItem(  "Debug Map",
                                            debugMap,
                                            true));
        }

        private void CreteLoadGameEntry()
        {
            MenuItem.MenuAction newGame = (pars) => {

                ActivateByName("Resume");
                currGame.CleanMsgs();
                currGame.ClearMaps();
                currGame.Load(@"current.game");
                currGame.GameInitialization();
                Close();
            };

            this.AddMenuItem(new MenuItem(  "Load Game",
                                            newGame,
                                            true));
        }

        private void CreateNewGameEntry()
        {
            MenuItem.MenuAction newGame = (pars) =>
            {
                currGame.ResetMapBuilder();
                currGame.CleanMsgs();
                var builder = currGame.MapBuilder;

                builder.Height = 40;
                builder.Width = 30;

                builder.Explored = TernaryValue.Unexplored;
                builder.Lightened = true;

                builder.MapCreationMode = MapBuilder.TableCreationMode.Random;

                ActivateByName("Resume");
                currGame.WorldReset();
                currGame.GameInitialization();
                Close();
            };

            this.AddMenuItem(new MenuItem(  "New Game",
                                            newGame,
                                            true));
        }
    }
}
