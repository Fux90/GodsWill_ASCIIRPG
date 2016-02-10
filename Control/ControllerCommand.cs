using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{	
    /// <summary>
    /// All commands a controller can be notified with
    /// </summary>
	public enum ControllerCommand
	{
        Player_MoveNorth,
        Player_MoveWest,
        Player_MoveSouth,
        Player_MoveEast,
        Player_ExitGame,
        Player_PickUp,
        Player_PutOn,
        Player_UnhandleWeapon,
        Player_PutOff,
        Player_HandleWeapon,
        Player_EmbraceShield,
        Player_PutAwayShield,
        Player_Pray,
        Player_ScrollMsgsUp,
        Player_ScrollMsgsDown,
        Player_EnterSelectionMode,
        Player_ExitSelectionModeWithoutSelection,
        Player_CastSpell,
        Player_UseItem,
        Player_SaveGame,
        Player_ActivateWeaponPower,
        Player_BackToMainMenu,
        Player_IsDead,
        Player_TriggerCurrent,

        Backpack_SelectNext,
        Backpack_SelectPrevious,
        Backpack_Close,
        Backpack_Pick,
        Backpack_Open,
        Backpack_SelectNextPage,
        Backpack_SelectPreviousPage,
        Backpack_UseItem,
        Backpack_PutOnArmor,
        Backpack_EmbraceShield,
        Backpack_HandleWeapon,

        AI_Turn,

        SelectionCursor_MoveNorth,
        SelectionCursor_MoveEast,
        SelectionCursor_MoveSouth,
        SelectionCursor_MoveWest,
        SelectionCursor_PickedCell,
        SelectionCursor_PickedCellOneOfMany,

        Spellbook_SelectPrevious,
        Spellbook_SelectPreviousPage,
        Spellbook_SelectNext,
        Spellbook_SelectNextPage,
        Spellbook_Pick,
        Spellbook_Close,
        Spellbook_Open,

        Menu_PreviousItem,
        Menu_NextItem,
        Menu_ExecuteSelectItem,

        Merchant_SelectNext,
        Merchant_SelectNextPage,
        Merchant_SelectPrevious,
        Merchant_SelectPreviousPage,
        Merchant_Open,
        Merchant_Close,
        Merchant_SelectPreviousList,
        Merchant_SelectNextList,
        Merchant_PurchaseSell,
    }
}