#define DEBUG_MAP

using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.AICharacters;
using GodsWill_ASCIIRPG.Model.Armors;
using GodsWill_ASCIIRPG.Model.SceneryItems;
using GodsWill_ASCIIRPG.Model.Shields;
using GodsWill_ASCIIRPG.Model.Weapons;
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

            public void InitialMenu(List<IAtomListener> atomListeners,
                                    List<ISheetViewer> sheetViews,
                                    List<IBackpackViewer> backpackViewers)
            {
                currentPg = new PgCreator() { God = Gods.Ares }.Create();
                atomListeners.ForEach( listener => currentPg.RegisterListener(listener));
                sheetViews.ForEach( sheet => currentPg.RegisterSheet(sheet));
                backpackViewers.ForEach(viewer => currentPg.Backpack.RegisterViewer(viewer));
            }

            public void GameInitialization( PgController pgController, 
                                            AIController aiController,
                                            IMapViewer mapViewer )
            {
                // Map generation
                var mapBuilder = new MapBuilder();
#if DEBUG_MAP
                mapBuilder.Height = 20;
                mapBuilder.Width = 20;
                mapBuilder.AddAtom(new Wall(new Coord() { X = 10, Y = 10 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 11, Y = 10 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 12, Y = 10 }));

                mapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = 0 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = mapBuilder.Height - 1 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = mapBuilder.Width - 1, Y = 0 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = mapBuilder.Width - 1, Y = mapBuilder.Height - 1 }));

                mapBuilder.AddViewer(mapViewer);
                mapBuilder.PlayerInitialPosition = new Coord() { X = 1, Y = 1 };
                mapBuilder.AddAtom(new LongSword(position: new Coord() { X = 7, Y = 4 }));
                mapBuilder.AddAtom(new Leather(position: new Coord() { X = 7, Y = 3 }));
                for (int i = 8; i < 18; i++)
                {
                    for (int j = 4; j < 14; j++)
                    {
                        mapBuilder.AddAtom(new LongSword(position: new Coord() { X = i, Y = j }));
                    }
                }
                mapBuilder.AddAtom(new WoodenShield(position: Coord.Random(mapBuilder.Width, mapBuilder.Height)));
                var orcBuilder = AICharacter.DummyCharacter(typeof(Orc)).Builder;
                orcBuilder.Position = new Coord(10, 12);
                var orc = orcBuilder.Build();
                mapBuilder.AddAtom(orc);
                currentPg.Listeners.ForEach(listener => orc.RegisterListener(listener));
                aiController.Register(orc);
#else
                mapBuilder.LoadFromFile("");
#endif
                var map = mapBuilder.Create();

                pgController.Register(CurrentPg);
                pgController.BackpackController.Register(CurrentPg.Backpack);

                CurrentPg.InsertInMap(map, map.PlayerInitialPosition);
            }

            private Game()
            {

            }
		}
	}
}