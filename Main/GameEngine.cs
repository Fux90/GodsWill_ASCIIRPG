using GodsWill_ASCIIRPG.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	namespace Main
	{
        class GameEngine
        {
            private Map currentMap;
            private PgController pgController;
            private AIController aiController;

            public Map CurrentMap { get { return currentMap; } }

            public GameEngine(  Map currentMap,
                                PgController pgController,
                                AIController aiController)
            {
                this.currentMap = currentMap;
                this.pgController = pgController;
                this.aiController = aiController;
            }

            public bool Turn()
            {
                return true;
            }
        }
    }
}