#define FROM_MAIN_MENU

#define DEBUG_MAP
//#define RANDOM_MAP_GENERATION
//#define DEBUG_MAP_FROM_FILE

using GodsWill_ASCIIRPG.Control;
using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.AICharacters;
using GodsWill_ASCIIRPG.Model.Armors;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
using GodsWill_ASCIIRPG.Model.Menus;
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
        enum MenuType
        {
            Main
        }

		public class Game
		{
            private static Game current;
            private Pg currentPg;
            public MapBuilder MapBuilder { get; private set; }

            public List<Map> Maps { get; private set; }
            public int CurrentMapIx { get; private set; }

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

            public Map CurrentMap
            {
                get
                {
                    return Game.Current.Maps[CurrentMapIx];
                }
            }

            private Dictionary<MenuType, IMenuViewer> menuViewers;
            private Dictionary<MenuType, MenuController> menuControllers;

            PgController pgController;
            AIController aiController;
            IMapViewer mapViewer;
            IAtomListener singleMsgListener;
            IAnimationViewer animationViewer;
            List<IAtomListener> atomListeners;
            List<ISheetViewer> sheetViews;
            List<IBackpackViewer> backpackViewers;
            List<ISpellbookViewer> spellbookViewers;
            List<IAtomListener> secondarySpellsViewers;

            public void Init(   PgController pgController,
                                AIController aiController,
                                IMapViewer mapViewer,
                                IAtomListener singleMsgListener,
                                IAnimationViewer animationViewer,
                                List<IAtomListener> atomListeners,
                                List<ISheetViewer> sheetViews,
                                List<IBackpackViewer> backpackViewers,
                                List<ISpellbookViewer> spellbookViewers,
                                List<IAtomListener> secondarySpellsViewers,
                                IMenuViewer mainMenuViewer,
                                MenuController mainMenuController)
            {
                this.pgController = pgController;
                this.aiController = aiController;
                this.mapViewer = mapViewer;
                this.singleMsgListener = singleMsgListener;
                this.animationViewer = animationViewer;
                this.atomListeners = atomListeners;
                this.sheetViews = sheetViews;
                this.backpackViewers = backpackViewers;
                this.spellbookViewers = spellbookViewers;
                this.secondarySpellsViewers = secondarySpellsViewers;

                menuViewers = new Dictionary<MenuType, IMenuViewer>();
                menuControllers = new Dictionary<MenuType, MenuController>();

                menuViewers[MenuType.Main] = mainMenuViewer;
                menuControllers[MenuType.Main] = mainMenuController;
            }

            public void InitialMenu(bool resumeActive = false)
            {
                var mainMenu = new MainMenu(this, resumeActive);

                mainMenu.RegisterView(menuViewers[MenuType.Main]);
                menuControllers[MenuType.Main].Register(mainMenu);

                mainMenu.Open();
            }

            public void ResetMapBuilder()
            {
                MapBuilder = new MapBuilder();

                var controllers = new List<Controller>()
                {
                    pgController,
                    aiController,
                };

                controllers.Where(controller => controller != null)
                            .ToList()
                            .ForEach(controller => controller.UnregisterAll());
            }

            public void PgCreation()
            {
                currentPg = new PgCreator() { God = Gods.Ares }.Create();
            }

#if FROM_MAIN_MENU
            public void DebugMap()
            {
                MapBuilder.Explored = TernaryValue.Unexplored;
                MapBuilder.Lightened = true;

                MapBuilder.Height = 30;
                MapBuilder.Width = 19;

                //mapBuilder.AddAtom(new Wall(new Coord() { X = 10, Y = 10 }));
                //mapBuilder.AddAtom(new Wall(new Coord() { X = 11, Y = 10 }));
                //mapBuilder.AddAtom(new Wall(new Coord() { X = 12, Y = 10 }));

                // Orc Covering Walls
                for (int x = 9; x < 12; x++)
                {
                    MapBuilder.AddAtom(new Wall(new Coord(x, 10)));
                }
                for (int y = 10; y < 14; y++)
                {
                    MapBuilder.AddAtom(new Wall(new Coord(8, y)));
                }
                // 4 Corners
                MapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = 0 }));
                MapBuilder.AddAtom(new Wall(new Coord() { X = 0, Y = MapBuilder.Height - 1 }));
                MapBuilder.AddAtom(new Wall(new Coord() { X = MapBuilder.Width - 1, Y = 0 }));
                MapBuilder.AddAtom(new Wall(new Coord() { X = MapBuilder.Width - 1, Y = MapBuilder.Height - 1 }));

                MapBuilder.PlayerInitialPosition = new Coord() { X = 1, Y = 1 };

                MapBuilder.AddAtom(new Gold(10, new Coord(2, 2)));
                MapBuilder.AddAtom(new LongSword(position: new Coord() { X = 7, Y = 4 }));
                MapBuilder.AddAtom(new Leather(position: new Coord() { X = 7, Y = 3 }));
                for (int i = 8; i < 18; i++)
                {
                    for (int j = 4; j < 14; j++)
                    {
                        if (i % 2 == 0)
                        {
                            MapBuilder.AddAtom(new LongSword(position: new Coord() { X = i, Y = j }));
                        }
                        else
                        {
                            MapBuilder.AddAtom(new FlamingLongSword(position: new Coord() { X = i, Y = j }));
                        }
                    }
                }

                var book1 = ItemGenerators.GenerateByBuilderType(typeof(PrayerBookBuilder), position: new Coord(2, 1));
                MapBuilder.AddAtom(book1);

                MapBuilder.AddAtom(new WoodenShield(position: Coord.Random(MapBuilder.Width, MapBuilder.Height)));
                var orcBuilder1 = AICharacter.DummyCharacter(typeof(Orc)).Builder;
                orcBuilder1.Position = new Coord(10, 12);
                var orc1 = orcBuilder1.Build();
                MapBuilder.AddAtom(orc1);


                var orcBuilder2 = AICharacter.DummyCharacter(typeof(Orc)).Builder;
                orcBuilder2.Position = new Coord(13, 12);
                var orc2 = orcBuilder2.Build();
                MapBuilder.AddAtom(orc2);

                currentPg = new PgCreator() { God = Gods.Ares }.Create();

                currentPg.Listeners.ForEach(listener => orc1.RegisterListener(listener));
                currentPg.Listeners.ForEach(listener => orc2.RegisterListener(listener));
            }

            public void Exit()
            {
                pgController.Notify(ControllerCommand.Player_ExitGame);
            }

            public void GameInitialization( //PgController pgController,           //                                AIController aiController,
            //                                IMapViewer mapViewer,
            //                                IAtomListener singleMsgListener,
            //                                IAnimationViewer animationViewer,
            //                                List<IAtomListener> atomListeners,
            //                                List<ISheetViewer> sheetViews,
            //                                List<IBackpackViewer> backpackViewers,
            //                                List<ISpellbookViewer> spellbookViewers,
            //                                List<IAtomListener> secondarySpellsViewers
            )
            {
                Maps.Clear();

                MapBuilder.AddViewer(mapViewer);
                MapBuilder.AddSingleMessageListener(singleMsgListener);

                var map = MapBuilder.Create();
//#if !DEBUG_MAP_FROM_FILE
//                if (currentPg == null)
//                {
//                    currentPg = new PgCreator() { God = Gods.Ares }.Create();
//                }
//                CurrentPg.InsertInMap(map, map.PlayerInitialPosition, overwrite: false);
//#else
                currentPg = map.CurrentPg;

                if (currentPg == null)
                {
                    PgCreation();
                    CurrentPg.InsertInMap(map, map.PlayerInitialPosition, overwrite: false);
                }
                else
                {
                    CurrentPg.InsertInMap(map, currentPg.Position, overwrite: true);
                }
//#endif
                var aiCharacters = map.AICharacters.ToList();

                atomListeners.ForEach(listener => currentPg.RegisterListener(listener));
                aiCharacters.ForEach(aiC => currentPg.Listeners.ForEach(listener => aiC.RegisterListener(listener)));
                sheetViews.ForEach(sheet => currentPg.RegisterSheet(sheet));
                backpackViewers.ForEach(viewer => currentPg.Backpack.RegisterViewer(viewer));
                spellbookViewers.ForEach(viewer => currentPg.Spellbook.RegisterViewer(viewer));
                secondarySpellsViewers.ForEach(secondarySpellsViewer => currentPg.Spellbook.RegisterSecondaryView(secondarySpellsViewer));

                pgController.Register(CurrentPg);
                pgController.BackpackController.Register(CurrentPg.Backpack);
                pgController.SpellbookController.Register(CurrentPg.Spellbook);

                aiController.RegisterAll(aiCharacters);
                Animation.RegisterAnimationViewer(animationViewer);
            }
#else
            public void GameInitialization( PgController pgController, 
                                            AIController aiController,
                                            IMapViewer mapViewer,
                                            IAtomListener singleMsgListener,
                                            IAnimationViewer animationViewer,
                                            List<IAtomListener> atomListeners,
                                            List<ISheetViewer> sheetViews,
                                            List<IBackpackViewer> backpackViewers,
                                            List<ISpellbookViewer> spellbookViewers,
                                            List<IAtomListener> secondarySpellsViewers)
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
#if !DEBUG_MAP_FROM_FILE
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
                        if (i % 2 == 0)
                        {
                            mapBuilder.AddAtom(new LongSword(position: new Coord() { X = i, Y = j }));
                        }
                        else
                        {
                            mapBuilder.AddAtom(new FlamingLongSword(position: new Coord() { X = i, Y = j }));
                        }
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

                currentPg = new PgCreator() { God = Gods.Ares }.Create();
                
                currentPg.Listeners.ForEach(listener => orc1.RegisterListener(listener));
                currentPg.Listeners.ForEach(listener => orc2.RegisterListener(listener));
                //aiController.Register(orc1);
                //aiController.Register(orc2);
#endif
#endif
#else
                mapBuilder.LoadFromFile("");
#endif
#if DEBUG_MAP_FROM_FILE
                mapBuilder.MapCreationMode = MapBuilder.TableCreationMode.FromFile;
#endif
                var map = mapBuilder.Create();
#if !DEBUG_MAP_FROM_FILE
                if (currentPg == null)
                {
                    currentPg = new PgCreator() { God = Gods.Ares }.Create();
                }
                CurrentPg.InsertInMap(map, map.PlayerInitialPosition, overwrite: false);
#else
                currentPg = map.CurrentPg;

                if (currentPg == null)
                {
                    currentPg = new PgCreator() { God = Gods.Ares }.Create();
                    CurrentPg.InsertInMap(map, map.PlayerInitialPosition, overwrite: false);
                }
                else
                {
                    CurrentPg.InsertInMap(map, currentPg.Position, overwrite: true);
                }
#endif
                var aiCharacters = map.AICharacters.ToList();

                atomListeners.ForEach(listener => currentPg.RegisterListener(listener));
                aiCharacters.ForEach(aiC => currentPg.Listeners.ForEach(listener => aiC.RegisterListener(listener)));
                sheetViews.ForEach(sheet => currentPg.RegisterSheet(sheet));
                backpackViewers.ForEach(viewer => currentPg.Backpack.RegisterViewer(viewer));
                spellbookViewers.ForEach(viewer => currentPg.Spellbook.RegisterViewer(viewer));
                secondarySpellsViewers.ForEach(secondarySpellsViewer => currentPg.Spellbook.RegisterSecondaryView(secondarySpellsViewer));

                pgController.Register(CurrentPg);
                pgController.BackpackController.Register(CurrentPg.Backpack);
                pgController.SpellbookController.Register(CurrentPg.Spellbook);

                aiController.RegisterAll(aiCharacters);
                Animation.RegisterAnimationViewer(animationViewer);
            }
#endif
            private Game()
            {
                Maps = new List<Map>();
                ResetMapBuilder();
            }
		}
	}
}