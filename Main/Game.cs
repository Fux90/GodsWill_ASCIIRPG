#define DEBUG_MAP
#define RANDOM_MAP_GENERATION

using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.AICharacters;
using GodsWill_ASCIIRPG.Model.Armors;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
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
                                    List<IBackpackViewer> backpackViewers,
                                    List<ISpellbookViewer> spellbookViewers,
                                    List<IAtomListener> secondarySpellsViewers)
            {
                currentPg = new PgCreator() { God = Gods.Ares }.Create();
                atomListeners.ForEach( listener => currentPg.RegisterListener(listener));
                sheetViews.ForEach( sheet => currentPg.RegisterSheet(sheet));
                backpackViewers.ForEach(viewer => currentPg.Backpack.RegisterViewer(viewer));
                spellbookViewers.ForEach(viewer => currentPg.Spellbook.RegisterViewer(viewer));
                secondarySpellsViewers.ForEach(secondarySpellsViewer => currentPg.Spellbook.RegisterSecondaryView(secondarySpellsViewer));
            }

            public void GameInitialization( PgController pgController, 
                                            AIController aiController,
                                            IMapViewer mapViewer,
                                            IAtomListener singleMsgListener,
                                            IAnimationViewer animationViewer)
            {
                // Map generation
                var mapBuilder = new MapBuilder();

                mapBuilder.AddViewer(mapViewer);
                mapBuilder.AddSingleMessageListener(singleMsgListener);

#if DEBUG_MAP
                mapBuilder.Explored = TernaryValue.Unexplored;
                mapBuilder.Lightened = true;
#if RANDOM_MAP_GENERATION
                mapBuilder.Height = 40;
                mapBuilder.Width = 30;

                mapBuilder.MapCreationMode = MapBuilder.TableCreationMode.Random;
#else
                mapBuilder.Height = 30;
                mapBuilder.Width = 19;

                //mapBuilder.AddAtom(new Wall(new Coord() { X = 10, Y = 10 }));
                //mapBuilder.AddAtom(new Wall(new Coord() { X = 11, Y = 10 }));
                //mapBuilder.AddAtom(new Wall(new Coord() { X = 12, Y = 10 }));

                // Orc Covering Walls
                for (int x = 9; x < 12; x++)
                {
                    mapBuilder.AddAtom(new Wall(new Coord(x, 10)));
                }
                for (int y = 10; y < 14; y++)
                {
                    mapBuilder.AddAtom(new Wall(new Coord(8, y)));
                }
                // 4 Corners
                mapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = 0 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = mapBuilder.Height - 1 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = mapBuilder.Width - 1, Y = 0 }));
                mapBuilder.AddAtom(new Wall(new Coord() { X = mapBuilder.Width - 1, Y = mapBuilder.Height - 1 }));

                mapBuilder.PlayerInitialPosition = new Coord() { X = 1, Y = 1 };

                mapBuilder.AddAtom(new Gold(10, new Coord(2, 2)));
                mapBuilder.AddAtom(new LongSword(position: new Coord() { X = 7, Y = 4 }));
                mapBuilder.AddAtom(new Leather(position: new Coord() { X = 7, Y = 3 }));
                for (int i = 8; i < 18; i++)
                {
                    for (int j = 4; j < 14; j++)
                    {
                        mapBuilder.AddAtom(new LongSword(position: new Coord() { X = i, Y = j }));
                    }
                }

                var book1 = ItemGenerators.GenerateByBuilderType(typeof(PrayerBookBuilder), position: new Coord(2, 1));
                mapBuilder.AddAtom(book1);

                mapBuilder.AddAtom(new WoodenShield(position: Coord.Random(mapBuilder.Width, mapBuilder.Height)));
                var orcBuilder1 = AICharacter.DummyCharacter(typeof(Orc)).Builder;
                orcBuilder1.Position = new Coord(10, 12);
                var orc1 = orcBuilder1.Build();
                mapBuilder.AddAtom(orc1);


                var orcBuilder2 = AICharacter.DummyCharacter(typeof(Orc)).Builder;
                orcBuilder2.Position = new Coord(13, 12);
                var orc2 = orcBuilder2.Build();
                mapBuilder.AddAtom(orc2);

                currentPg.Listeners.ForEach(listener => orc1.RegisterListener(listener));
                currentPg.Listeners.ForEach(listener => orc2.RegisterListener(listener));
                aiController.Register(orc1);
                aiController.Register(orc2);
#endif
#else
                mapBuilder.LoadFromFile("");
#endif
                mapBuilder.MapCreationMode = MapBuilder.TableCreationMode.FromFile;
                var map = mapBuilder.Create();

                CurrentPg.InsertInMap(map, map.PlayerInitialPosition);

                pgController.Register(CurrentPg);
                pgController.BackpackController.Register(CurrentPg.Backpack);
                pgController.SpellbookController.Register(CurrentPg.Spellbook);

                

                Animation.RegisterAnimationViewer(animationViewer);
            }

            private Game()
            {

            }
		}
	}
}