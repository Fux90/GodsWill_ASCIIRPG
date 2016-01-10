#define DEBUG_MAP

using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.SceneryItems;
using GodsWill_ASCIIRPG.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	namespace Main
	{
		class Game
		{
            private static Game current;
            private Pg currentPg;
            private GameEngine gameEngine;

            public static Game Current
            {
                get
                {
                    if (current == null)
                    {
                        current = new Game();
                    }

                    return current;
                }
            }

            public Pg CurrentPg
            {
                get
                {
                    return Game.Current.currentPg;
                }
            }

            public void Init()
            {

            }

            public void InitialMenu(IAtomListener[] atomListeners,
                                    ISheetViewer[] sheetViews)
            {
                currentPg = new PgCreator().Create();
                foreach (var listener in atomListeners)
                {
                    currentPg.RegisterListener(listener);
                }
                foreach (var sheet in sheetViews)
                {
                    currentPg.RegisterSheet(sheet);
                }
            }

            public void GameInitialization( PgController pgController, IMapViewer mapViewer )
            {
                // Map generation
                var mapBuilder = new MapBuilder();
#if DEBUG_MAP
                mapBuilder.Height = 20;
                mapBuilder.Width = 20;
                mapBuilder.AddAtom(new Wall(new Coord() { X = 10, Y = 10 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 11, Y = 10 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 12, Y = 10 }));
                mapBuilder.AddViewer(mapViewer);
                mapBuilder.PlayerInitialPosition = new Coord() { X = 1, Y = 1 };
#else
                mapBuilder.LoadFromFile("");
#endif
                var map = mapBuilder.Create();

                pgController.Register(CurrentPg);
                pgController.BackpackController.Register(CurrentPg.Backpack);

                AIController aiController = null;
                CurrentPg.InsertInMap(map, map.PlayerInitialPosition);

                // Collocamento giocatore
                gameEngine = new GameEngine( map,
                                             pgController,
                                             aiController);
            }

            public void RunGame()
            {
                while (gameEngine.Turn()) ;
            }

            private Game()
            {

            }
		}
	}
}