using MS_EntWatch.Helpers;
using MS_EntWatch.Items;
using Sharp.Shared.Enums;
using Sharp.Shared.Types;

namespace MS_EntWatch
{
    public partial class EntWatch
    {
        void RegMapCommands()
        {
            _convars.CreateServerCommand("ew_setcooldown", OnEWMC_SetCooldown, "Allows you to change the item’s cooldown during the game", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setmaxuses", OnEWMC_SetMaxUses, "Allows you to change the maximum use of the item during the game, depending on whether the item was used to the end", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setuses", OnEWMC_SetUses, "Allows you to change the current use of the item during the game, depending on whether the item was used to the end", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_addmaxuses", OnEWMC_AddMaxUses, "Allows you to add 1 charge to the item, depending on whether the item was used to the end", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setmode", OnEWMC_SetMode, "Allows you to completely change the item", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_lockbutton", OnEWMC_LockButton, "Allows to lock item", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setabilityname", OnEWMC_SetAbilityName, "Allows you to change the ability’s name", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setname", OnEWMC_SetFullName, "Allows you to change the item’s name(Chat)", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_setshortname", OnEWMC_SetShortName, "Allows you to change the item’s shortname(HUD)", ConVarFlags.Hidden);
            _convars.CreateServerCommand("ew_block", OnEWMC_Block, "Allows you to block an item during the game. Similar to the 'blockpickup' property", ConVarFlags.Hidden);
        }

        void UnRegMapCommands()
        {
            _convars.ReleaseCommand("ew_setcooldown");
            _convars.ReleaseCommand("ew_setmaxuses");
            _convars.ReleaseCommand("ew_setuses");
            _convars.ReleaseCommand("ew_addmaxuses");
            _convars.ReleaseCommand("ew_setmode");
            _convars.ReleaseCommand("ew_lockbutton");
            _convars.ReleaseCommand("ew_setabilityname");
            _convars.ReleaseCommand("ew_setname");
            _convars.ReleaseCommand("ew_setshortname");
            _convars.ReleaseCommand("ew_block");
        }

        private ECommandAction OnEWMC_SetCooldown(StringCommand command) //<hammerid> <buttonid> <new cooldown> [<force apply>]
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 3) return ECommandAction.Stopped;
            MCS_ValueIntBool Value = new();
            if (!Int32.TryParse(command.GetArg(3), out Value.iVal) || Value.iVal < 0) return ECommandAction.Stopped;

            Value.bFlag = false;
            string sFlag = command.ArgCount >= 4 ? command.GetArg(4) : "";
            if (!string.IsNullOrEmpty(sFlag)) Value.bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<MCS_ValueIntBool>.ForAllAbilities(command, MapCommands<MCS_ValueIntBool>.DG_SetCoolDown, Value);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetMaxUses(StringCommand command) //<hammerid> <buttonid> <maxuses> [<even if over>]
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 3) return ECommandAction.Stopped;
            MCS_ValueIntBool Value = new();
            if (!Int32.TryParse(command.GetArg(3), out Value.iVal) || Value.iVal < 0) return ECommandAction.Stopped;

            Value.bFlag = false;
            string sFlag = command.ArgCount >= 4 ? command.GetArg(4) : "";
            if (!string.IsNullOrEmpty(sFlag)) Value.bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<MCS_ValueIntBool>.ForAllAbilities(command, MapCommands<MCS_ValueIntBool>.DG_SetMaxUses, Value);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetUses(StringCommand command) //<hammerid> <buttonid> <value> [<even if over>]
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 3) return ECommandAction.Stopped;
            MCS_ValueIntBool Value = new();
            if (!Int32.TryParse(command.GetArg(3), out Value.iVal) || Value.iVal < 0) return ECommandAction.Stopped;

            Value.bFlag = false;
            string sFlag = command.ArgCount >= 4 ? command.GetArg(4) : "";
            if (!string.IsNullOrEmpty(sFlag)) Value.bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<MCS_ValueIntBool>.ForAllAbilities(command, MapCommands<MCS_ValueIntBool>.DG_SetUses, Value);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_AddMaxUses(StringCommand command) //<hammerid> <buttonid> [<even if over>]
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 2) return ECommandAction.Stopped;

            bool bFlag = false;
            string sFlag = command.ArgCount >= 3 ? command.GetArg(3) : "";
            if (!string.IsNullOrEmpty(sFlag)) bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<bool>.ForAllAbilities(command, MapCommands<bool>.DG_AddMaxUses, bFlag);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetMode(StringCommand command) //<hammerid> <buttonid> <newmode> <cooldown> <maxuses> [<even if over>]
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 5) return ECommandAction.Stopped;
            MCS_ValueMode Value = new();
            if (!Int32.TryParse(command.GetArg(3), out Value.iMode) || Value.iMode < 0 || Value.iMode > 8) return ECommandAction.Stopped;
            if (!Int32.TryParse(command.GetArg(4), out Value.iCooldown) || Value.iCooldown < 0) return ECommandAction.Stopped;
            if (!Int32.TryParse(command.GetArg(5), out Value.iMaxuses) || Value.iMaxuses < 0) return ECommandAction.Stopped;

            Value.bFlag = false;
            string sFlag = command.ArgCount >= 6 ? command.GetArg(6) : "";
            if (!string.IsNullOrEmpty(sFlag)) Value.bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<MCS_ValueMode>.ForAllAbilities(command, MapCommands<MCS_ValueMode>.DG_SetMode, Value);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_LockButton(StringCommand command) //<hammerid> <buttonid> <value>
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 3) return ECommandAction.Stopped;

            bool bFlag = false;
            string sFlag = command.GetArg(3);
            if (!string.IsNullOrEmpty(sFlag)) bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<bool>.ForAllAbilities(command, MapCommands<bool>.DG_LockButton, bFlag);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetAbilityName(StringCommand command) //<hammerid> <buttonid> <newname>
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 3) return ECommandAction.Stopped;

            string sNewName = command.GetArg(3);
            if (string.IsNullOrEmpty(sNewName)) return ECommandAction.Stopped;

            MapCommands<string>.ForAllAbilities(command, MapCommands<string>.DG_SetAbilityName, sNewName);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetFullName(StringCommand command) //<hammerid> <newname>
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 2) return ECommandAction.Stopped;

            string sNewName = command.GetArg(2);
            if (string.IsNullOrEmpty(sNewName)) return ECommandAction.Stopped;

            MapCommands<string>.ForAllItems(command, MapCommands<string>.DG_SetFullName, sNewName);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_SetShortName(StringCommand command) //<hammerid> <newshortname>
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 2) return ECommandAction.Stopped;

            string sNewName = command.GetArg(2);
            if (string.IsNullOrEmpty(sNewName)) return ECommandAction.Stopped;

            MapCommands<string>.ForAllItems(command, MapCommands<string>.DG_SetShortName, sNewName);

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWMC_Block(StringCommand command) //<hammerid> <value>
        {
            if (!EW.g_CfgLoaded || command.ArgCount < 2) return ECommandAction.Stopped;

            bool bFlag = false;
            string sFlag = command.GetArg(2);
            if (!string.IsNullOrEmpty(sFlag)) bFlag = sFlag.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sFlag, "1");

            MapCommands<bool>.ForAllItems(command, MapCommands<bool>.DG_Block, bFlag);

            return ECommandAction.Stopped;
        }
    }

    delegate void MapCommAbilityFunc<T>(Ability AbilityTest, T Value);
    delegate void MapCommItemFunc<T>(Item ItemTest, T Value);
    static class MapCommands<T>
    {
        public static void ForAllAbilities(StringCommand command, MapCommAbilityFunc<T> mc_func, T Value)
        {
            if (command.ArgCount < 1) return;
            string sHammerID = command.GetArg(1);
            if (string.IsNullOrEmpty(sHammerID)) return;

            string sButtonID = command.ArgCount >= 2 ? command.GetArg(2) : "";

            EW.UpdateTime();

            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (string.Equals(ItemTest.HammerID, sHammerID, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                    {
                        if (string.Equals(sButtonID, AbilityTest.ButtonID) || string.IsNullOrEmpty(sButtonID) || string.Equals(sButtonID, "0")) mc_func(AbilityTest, Value);
                    }
                }
            }
        }

        public static void ForAllItems(StringCommand command, MapCommItemFunc<T> mc_func, T Value)
        {
            if (command.ArgCount < 1) return;
            string sHammerID = command.GetArg(1);
            if (string.IsNullOrEmpty(sHammerID)) return;

            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (string.Equals(sHammerID, ItemTest.HammerID, StringComparison.OrdinalIgnoreCase))
                {
                    mc_func(ItemTest, Value);
                }
            }
        }

        public static MapCommAbilityFunc<MCS_ValueIntBool> DG_SetCoolDown = (AbilityTest, Value) =>
        {
            AbilityTest.CoolDown = Value.iVal;
            if (Value.bFlag) AbilityTest.fLastUse = EW.fGameTime + AbilityTest.CoolDown;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetCooldown", 4, AbilityTest.ButtonID, Value.iVal, Value.bFlag);
        };

        public static MapCommAbilityFunc<MCS_ValueIntBool> DG_SetMaxUses = (AbilityTest, Value) =>
        {
            if (AbilityTest.MaxUses > AbilityTest.iCurrentUses || Value.bFlag)
            {
                AbilityTest.MaxUses = Value.iVal;
                if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.MaxUses", 4, AbilityTest.ButtonID, Value.iVal, Value.bFlag);
            }
        };

        public static MapCommAbilityFunc<MCS_ValueIntBool> DG_SetUses = (AbilityTest, Value) =>
        {
            if (AbilityTest.MaxUses > AbilityTest.iCurrentUses || Value.bFlag)
            {
                AbilityTest.iCurrentUses = Value.iVal;
                if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetCurrentUses", 4, AbilityTest.ButtonID, Value.iVal, Value.bFlag);
            }
        };

        public static MapCommAbilityFunc<bool> DG_AddMaxUses = (AbilityTest, Value) =>
        {
            if (AbilityTest.MaxUses > AbilityTest.iCurrentUses || Value)
            {
                AbilityTest.MaxUses++;
                if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.AddMaxUses", 4, AbilityTest.ButtonID, Value);
            }
        };

        public static MapCommAbilityFunc<MCS_ValueMode> DG_SetMode = (AbilityTest, Value) =>
        {
            if (AbilityTest.MaxUses > AbilityTest.iCurrentUses || Value.bFlag || Value.iMode == 7 || Value.iMode == 6 || Value.iMode == 2 || Value.iMode == 1)
            {
                AbilityTest.Mode = Value.iMode;
                AbilityTest.CoolDown = Value.iCooldown;
                AbilityTest.MaxUses = Value.iMaxuses;
                AbilityTest.SetSpawnedMath();
                if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetMode", 4, AbilityTest.ButtonID, Value.iMode, Value.iCooldown, Value.iMaxuses, Value.bFlag);
            }
        };

        public static MapCommAbilityFunc<bool> DG_LockButton = (AbilityTest, Value) =>
        {
            AbilityTest.LockItem = Value;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.LockButton", 4, AbilityTest.ButtonID, Value);
        };

        public static MapCommAbilityFunc<string> DG_SetAbilityName = (AbilityTest, Value) =>
        {
            AbilityTest.Name = Value;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetAbilityName", 4, AbilityTest.ButtonID, Value);
        };

        public static MapCommItemFunc<string> DG_SetFullName = (ItemTest, Value) =>
        {
            ItemTest.Name = Value;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetFullName", 4, ItemTest.HammerID, Value);
        };

        public static MapCommItemFunc<string> DG_SetShortName = (ItemTest, Value) =>
        {
            ItemTest.ShortName = Value;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.SetShortName", 4, ItemTest.HammerID, Value);
        };

        public static MapCommItemFunc<bool> DG_Block = (ItemTest, Value) =>
        {
            ItemTest.BlockPickup = Value;
            if (ItemTest.WeaponHandle is { IsValidEntity:true, IsWeapon: true } weapon) weapon.CanBePickedUp = !Value;
            if (Cvar.DisplayMapCommands) UI.EWSysInfo("EntWatch.Info.MapCommands.Block", 4, ItemTest.HammerID, Value);
        };
    }

    struct MCS_ValueIntBool
    {
        public int iVal;
        public bool bFlag;
    }

    struct MCS_ValueMode
    {
        public int iMode;
        public int iCooldown;
        public int iMaxuses;
        public bool bFlag;
    }
}
