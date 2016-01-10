using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
	public enum ControllerCommand
	{
        Player_MoveNorth,
        Player_MoveWest,
        Player_MoveSouth,
        Player_MoveEast,
        Player_ExitGame,
        Player_PickUp,
        Backpack_SelectNext,
        Backpack_SelectPrevious,
        Backpack_Close,
        Backpack_Pick,
        Backpack_Open,
    }
}