using MS_EntWatch.Helpers;
using MS_EntWatch.Items;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace MS_EntWatch
{
    public partial class EntWatch
    {
        IConVar? g_Cvar_TeamOnly,
            g_Cvar_AdminChat,
            g_Cvar_AdminHud,
            g_Cvar_PlayerFormat,
            g_Cvar_BlockEPickup,
            g_Cvar_Delay,
            g_Cvar_GlobalBlock,
            g_Cvar_DisplayAbility,
            g_Cvar_UsePriority,
            g_Cvar_DisplayMapCommands,
            g_Cvar_SchemeName,
            g_Cvar_LowerMapname,
            g_Cvar_GlowSpawn,
            g_Cvar_GlowParticle,
            g_Cvar_GlowProp,
            g_Cvar_GlowVIP,
            g_Cvar_BanTime,
            g_Cvar_BanLong,
            g_Cvar_BanReason,
            g_Cvar_UnBanReason,
            g_Cvar_KeepExpiredBan,
            g_Cvar_OfflineClearTime,
            g_Cvar_ClanTag,
            g_Cvar_ClanTagInfo,
            g_Cvar_RemoveItemAfterRoundEnd,
            g_Cvar_ServerLanguage;

        private void RegisterCvars()
        {
            g_Cvar_TeamOnly = _convars.CreateConVar("ms_ewc_teamonly", true, "Enable/Disable team only mode", ConVarFlags.Notify);
            g_Cvar_AdminChat = _convars.CreateConVar("ms_ewc_adminchat", 0, 0, 2, "Change Admin Chat Mode (0 - All Messages, 1 - Only Pickup/Drop Items, 2 - Nothing)", ConVarFlags.Notify);
            g_Cvar_AdminHud = _convars.CreateConVar("ms_ewc_adminhud", 0, 0, 2, "Change Admin Hud Mode (0 - All Items, 1 - Only Item Name, 2 - Nothing)", ConVarFlags.Notify);
            g_Cvar_PlayerFormat = _convars.CreateConVar("ms_ewc_player_format", 3, 0, 3, "Changes the way player information is displayed by default (0 - Only Nickname, 1 - Nickname and UserID, 2 - Nickname and SteamID, 3 - Nickname, UserID and SteamID)", ConVarFlags.Notify);

            g_Cvar_BlockEPickup = _convars.CreateConVar("ms_ewc_blockepick", true, "Block players from using E key to grab items", ConVarFlags.Notify);
            g_Cvar_Delay = _convars.CreateConVar("ms_ewc_delay_use", 1.0f, 0.0f, 60.0f, "Change delay before use", ConVarFlags.Notify);
            g_Cvar_GlobalBlock = _convars.CreateConVar("ms_ewc_globalblock", false, "Blocks the pickup of any items by players", ConVarFlags.Notify);
            g_Cvar_DisplayAbility = _convars.CreateConVar("ms_ewc_display_ability", 4, 0, 4, "Count of the abilities to display on the HUD", ConVarFlags.Notify);
            g_Cvar_UsePriority = _convars.CreateConVar("ms_ewc_use_priority", true, "Enable/Disable forced pressing of the button", ConVarFlags.Notify);
            g_Cvar_DisplayMapCommands = _convars.CreateConVar("ms_ewc_display_mapcommands", true, "Enable/Disable display of item changes", ConVarFlags.Notify);

            g_Cvar_SchemeName = _convars.CreateConVar("ms_ewc_scheme_name", "default.json", "Filename for the scheme", ConVarFlags.None);
            g_Cvar_LowerMapname = _convars.CreateConVar("ms_ewc_lower_mapname", false, "Automatically lowercase map name", ConVarFlags.None);

            g_Cvar_GlowSpawn = _convars.CreateConVar("ms_ewc_glow_spawn", false, "Enable/Disable the glow after Spawn Items", ConVarFlags.Notify);
            g_Cvar_GlowParticle = _convars.CreateConVar("ms_ewc_glow_particle", false, "Enable/Disable the glow using a particle", ConVarFlags.Notify);
            g_Cvar_GlowProp = _convars.CreateConVar("ms_ewc_glow_prop", true, "Enable/Disable the glow using a prop_dynamic", ConVarFlags.Notify);
            g_Cvar_GlowVIP = _convars.CreateConVar("ms_ewc_glow_vip", false, "Enable/Disable the glow for privileged users", ConVarFlags.Notify);

            g_Cvar_BanTime = _convars.CreateConVar("ms_ewc_bantime", 0, 0, 43200, "Default ban time. 0 - Permanent", ConVarFlags.None);
            g_Cvar_BanLong = _convars.CreateConVar("ms_ewc_banlong", 720, 1, 1440000, "Max ban time with once `ew_ban` privilege", ConVarFlags.None);
            g_Cvar_BanReason = _convars.CreateConVar("ms_ewc_banreason", "Trolling", "Default ban reason", ConVarFlags.None);
            g_Cvar_UnBanReason = _convars.CreateConVar("ms_ewc_unbanreason", "Giving another chance", "Default unban reason", ConVarFlags.None);
            g_Cvar_KeepExpiredBan = _convars.CreateConVar("ms_ewc_keep_expired_ban", true, "Enable/Disable keep expired bans", ConVarFlags.None);
            g_Cvar_OfflineClearTime = _convars.CreateConVar("ms_ewc_offline_clear_time", 30, 1, 240, "Time during which data is stored (1-240)", ConVarFlags.None);

            g_Cvar_ClanTag = _convars.CreateConVar("ms_ewc_clantag", true, "Enable/Disable to display in the ClanTag", ConVarFlags.None);
            g_Cvar_ClanTagInfo = _convars.CreateConVar("ms_ewc_clantag_info", true, "Enable/Disable to display cooldown and other in the ClanTag", ConVarFlags.None);

            g_Cvar_RemoveItemAfterRoundEnd = _convars.CreateConVar("ms_ewc_endround_remove", true, "Enable/Disable to remove weapons after the end of the round", ConVarFlags.None);
            g_Cvar_ServerLanguage = _convars.CreateConVar("ms_ewc_server_lang", "en-us", "Specify the language into which the server messages should be translated", ConVarFlags.None);

            if (g_Cvar_TeamOnly != null) _convars.InstallChangeHook(g_Cvar_TeamOnly, OnCvarChanged_TeamOnly);
            if (g_Cvar_AdminChat != null) _convars.InstallChangeHook(g_Cvar_AdminChat, OnCvarChanged_AdminChat);
            if (g_Cvar_AdminHud != null) _convars.InstallChangeHook(g_Cvar_AdminHud, OnCvarChanged_AdminHud);
            if (g_Cvar_PlayerFormat != null) _convars.InstallChangeHook(g_Cvar_PlayerFormat, OnCvarChanged_PlayerFormat);

            if (g_Cvar_BlockEPickup != null) _convars.InstallChangeHook(g_Cvar_BlockEPickup, OnCvarChanged_BlockEPickup);
            if (g_Cvar_Delay != null) _convars.InstallChangeHook(g_Cvar_Delay, OnCvarChanged_Delay);
            if (g_Cvar_GlobalBlock != null) _convars.InstallChangeHook(g_Cvar_GlobalBlock, OnCvarChanged_GlobalBlock);
            if (g_Cvar_DisplayAbility != null) _convars.InstallChangeHook(g_Cvar_DisplayAbility, OnCvarChanged_DisplayAbility);
            if (g_Cvar_UsePriority != null) _convars.InstallChangeHook(g_Cvar_UsePriority, OnCvarChanged_UsePriority);
            if (g_Cvar_DisplayMapCommands != null) _convars.InstallChangeHook(g_Cvar_DisplayMapCommands, OnCvarChanged_DisplayMapCommands);

            if (g_Cvar_SchemeName != null) _convars.InstallChangeHook(g_Cvar_SchemeName, OnCvarChanged_SchemeName);
            if (g_Cvar_LowerMapname != null) _convars.InstallChangeHook(g_Cvar_LowerMapname, OnCvarChanged_LowerMapname);

            if (g_Cvar_GlowSpawn != null) _convars.InstallChangeHook(g_Cvar_GlowSpawn, OnCvarChanged_GlowSpawn);
            if (g_Cvar_GlowParticle != null) _convars.InstallChangeHook(g_Cvar_GlowParticle, OnCvarChanged_GlowParticle);
            if (g_Cvar_GlowProp != null) _convars.InstallChangeHook(g_Cvar_GlowProp, OnCvarChanged_GlowProp);
            if (g_Cvar_GlowVIP != null) _convars.InstallChangeHook(g_Cvar_GlowVIP, OnCvarChanged_GlowVIP);

            if (g_Cvar_BanTime != null) _convars.InstallChangeHook(g_Cvar_BanTime, OnCvarChanged_BanTime);
            if (g_Cvar_BanLong != null) _convars.InstallChangeHook(g_Cvar_BanLong, OnCvarChanged_BanLong);
            if (g_Cvar_BanReason != null) _convars.InstallChangeHook(g_Cvar_BanReason, OnCvarChanged_BanReason);
            if (g_Cvar_UnBanReason != null) _convars.InstallChangeHook(g_Cvar_UnBanReason, OnCvarChanged_UnBanReason);
            if (g_Cvar_KeepExpiredBan != null) _convars.InstallChangeHook(g_Cvar_KeepExpiredBan, OnCvarChanged_KeepExpiredBan);
            if (g_Cvar_OfflineClearTime != null) _convars.InstallChangeHook(g_Cvar_OfflineClearTime, OnCvarChanged_OfflineClearTime);

            if (g_Cvar_ClanTag != null) _convars.InstallChangeHook(g_Cvar_ClanTag, OnCvarChanged_ClanTag);
            if (g_Cvar_ClanTagInfo != null) _convars.InstallChangeHook(g_Cvar_ClanTagInfo, OnCvarChanged_ClanTagInfo);

            if (g_Cvar_RemoveItemAfterRoundEnd != null) _convars.InstallChangeHook(g_Cvar_RemoveItemAfterRoundEnd, OnCvarChanged_RemoveItemAfterRoundEnd);

            if (g_Cvar_ServerLanguage != null) _convars.InstallChangeHook(g_Cvar_ServerLanguage, OnCvarChanged_ServerLanguage);
        }

        private void UnRegisterCvars()
        {
            if (g_Cvar_TeamOnly != null) _convars.RemoveChangeHook(g_Cvar_TeamOnly, OnCvarChanged_TeamOnly);
            if (g_Cvar_AdminChat != null) _convars.RemoveChangeHook(g_Cvar_AdminChat, OnCvarChanged_AdminChat);
            if (g_Cvar_AdminHud != null) _convars.RemoveChangeHook(g_Cvar_AdminHud, OnCvarChanged_AdminHud);
            if (g_Cvar_PlayerFormat != null) _convars.RemoveChangeHook(g_Cvar_PlayerFormat, OnCvarChanged_PlayerFormat);

            if (g_Cvar_BlockEPickup != null) _convars.RemoveChangeHook(g_Cvar_BlockEPickup, OnCvarChanged_BlockEPickup);
            if (g_Cvar_Delay != null) _convars.RemoveChangeHook(g_Cvar_Delay, OnCvarChanged_Delay);
            if (g_Cvar_GlobalBlock != null) _convars.RemoveChangeHook(g_Cvar_GlobalBlock, OnCvarChanged_GlobalBlock);
            if (g_Cvar_DisplayAbility != null) _convars.RemoveChangeHook(g_Cvar_DisplayAbility, OnCvarChanged_DisplayAbility);
            if (g_Cvar_UsePriority != null) _convars.RemoveChangeHook(g_Cvar_UsePriority, OnCvarChanged_UsePriority);
            if (g_Cvar_DisplayMapCommands != null) _convars.RemoveChangeHook(g_Cvar_DisplayMapCommands, OnCvarChanged_DisplayMapCommands);

            if (g_Cvar_SchemeName != null) _convars.RemoveChangeHook(g_Cvar_SchemeName, OnCvarChanged_SchemeName);
            if (g_Cvar_LowerMapname != null) _convars.RemoveChangeHook(g_Cvar_LowerMapname, OnCvarChanged_LowerMapname);

            if (g_Cvar_GlowSpawn != null) _convars.RemoveChangeHook(g_Cvar_GlowSpawn, OnCvarChanged_GlowSpawn);
            if (g_Cvar_GlowParticle != null) _convars.RemoveChangeHook(g_Cvar_GlowParticle, OnCvarChanged_GlowParticle);
            if (g_Cvar_GlowProp != null) _convars.RemoveChangeHook(g_Cvar_GlowProp, OnCvarChanged_GlowProp);
            if (g_Cvar_GlowVIP != null) _convars.RemoveChangeHook(g_Cvar_GlowVIP, OnCvarChanged_GlowVIP);

            if (g_Cvar_BanTime != null) _convars.RemoveChangeHook(g_Cvar_BanTime, OnCvarChanged_BanTime);
            if (g_Cvar_BanLong != null) _convars.RemoveChangeHook(g_Cvar_BanLong, OnCvarChanged_BanLong);
            if (g_Cvar_BanReason != null) _convars.RemoveChangeHook(g_Cvar_BanReason, OnCvarChanged_BanReason);
            if (g_Cvar_UnBanReason != null) _convars.RemoveChangeHook(g_Cvar_UnBanReason, OnCvarChanged_UnBanReason);
            if (g_Cvar_KeepExpiredBan != null) _convars.RemoveChangeHook(g_Cvar_KeepExpiredBan, OnCvarChanged_KeepExpiredBan);
            if (g_Cvar_OfflineClearTime != null) _convars.RemoveChangeHook(g_Cvar_OfflineClearTime, OnCvarChanged_OfflineClearTime);

            if (g_Cvar_ClanTag != null) _convars.RemoveChangeHook(g_Cvar_ClanTag, OnCvarChanged_ClanTag);
            if (g_Cvar_ClanTagInfo != null) _convars.RemoveChangeHook(g_Cvar_ClanTagInfo, OnCvarChanged_ClanTagInfo);

            if (g_Cvar_RemoveItemAfterRoundEnd != null) _convars.RemoveChangeHook(g_Cvar_RemoveItemAfterRoundEnd, OnCvarChanged_RemoveItemAfterRoundEnd);

            if (g_Cvar_ServerLanguage != null) _convars.RemoveChangeHook(g_Cvar_ServerLanguage, OnCvarChanged_ServerLanguage);
        }

        private void OnCvarChanged_TeamOnly(IConVar conVar)
        {
            Cvar.TeamOnly = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.TeamOnly.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_AdminChat(IConVar conVar)
        {
            Cvar.AdminChat = conVar.GetInt16();
            UI.CvarChangeNotify(conVar.Name, Cvar.AdminChat.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_AdminHud(IConVar conVar)
        {
            Cvar.AdminHud = conVar.GetInt16();
            UI.CvarChangeNotify(conVar.Name, Cvar.AdminHud.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_PlayerFormat(IConVar conVar)
        {
            Cvar.PlayerFormat = conVar.GetInt16();
            UI.CvarChangeNotify(conVar.Name, Cvar.PlayerFormat.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_BlockEPickup(IConVar conVar)
        {
            Cvar.BlockEPickup = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.BlockEPickup.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_Delay(IConVar conVar)
        {
            Cvar.Delay = conVar.GetFloat();
            UI.CvarChangeNotify(conVar.Name, Cvar.Delay.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_GlobalBlock(IConVar conVar)
        {
            Cvar.GlobalBlock = conVar.GetBool();
            foreach (Item ItemTest in EW.g_ItemList.ToList()) ItemTest.WeaponHandle.CanBePickedUp = Cvar.GlobalBlock;
            UI.CvarChangeNotify(conVar.Name, Cvar.GlobalBlock.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_DisplayAbility(IConVar conVar)
        {
            Cvar.DisplayAbility = conVar.GetInt16();
            UI.CvarChangeNotify(conVar.Name, Cvar.DisplayAbility.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_UsePriority(IConVar conVar)
        {
            Cvar.UsePriority = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.UsePriority.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_DisplayMapCommands(IConVar conVar)
        {
            Cvar.DisplayMapCommands = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.DisplayMapCommands.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_SchemeName(IConVar conVar)
        {
            var value = conVar.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                Cvar.SchemeName = value.Replace("\"", "");
                UI.CvarChangeNotify(conVar.Name, Cvar.SchemeName, conVar.Flags.HasFlag(ConVarFlags.Notify));
            }
        }

        private void OnCvarChanged_LowerMapname(IConVar conVar)
        {
            Cvar.LowerMapname = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.LowerMapname.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_GlowSpawn(IConVar conVar)
        {
            Cvar.GlowSpawn = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.GlowSpawn.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_GlowParticle(IConVar conVar)
        {
            Cvar.GlowParticle = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.GlowParticle.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_GlowProp(IConVar conVar)
        {
            Cvar.GlowProp = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.GlowProp.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_GlowVIP(IConVar conVar)
        {
            Cvar.GlowVIP = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.GlowVIP.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_BanTime(IConVar conVar)
        {
            int value = conVar.GetInt32();
            if (value >= 0 && value <= 43200) Cvar.BanTime = value;
            else Cvar.BanTime = 0;
            UI.CvarChangeNotify(conVar.Name, Cvar.BanTime.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_BanLong(IConVar conVar)
        {
            int value = conVar.GetInt32();
            if (value >= 1 && value <= 1440000) Cvar.BanLong = value;
            else Cvar.BanLong = 720;
            UI.CvarChangeNotify(conVar.Name, Cvar.BanLong.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_BanReason(IConVar conVar)
        {
            var value = conVar.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                Cvar.BanReason = value.Replace("\"", "");
                UI.CvarChangeNotify(conVar.Name, Cvar.BanReason, conVar.Flags.HasFlag(ConVarFlags.Notify));
            }
        }

        private void OnCvarChanged_UnBanReason(IConVar conVar)
        {
            var value = conVar.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                Cvar.UnBanReason = value.Replace("\"", "");
                UI.CvarChangeNotify(conVar.Name, Cvar.UnBanReason, conVar.Flags.HasFlag(ConVarFlags.Notify));
            }
        }

        private void OnCvarChanged_KeepExpiredBan(IConVar conVar)
        {
            Cvar.KeepExpiredBan = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.KeepExpiredBan.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_OfflineClearTime(IConVar conVar)
        {
            int value = conVar.GetInt32();
            if (value >= 1 && value <= 240) Cvar.OfflineClearTime = value;
            else Cvar.OfflineClearTime = 30;
            UI.CvarChangeNotify(conVar.Name, Cvar.OfflineClearTime.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_ClanTag(IConVar conVar)
        {
            Cvar.ClanTag = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.ClanTag.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_ClanTagInfo(IConVar conVar)
        {
            Cvar.ClanTagInfo = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.ClanTagInfo.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_RemoveItemAfterRoundEnd(IConVar conVar)
        {
            Cvar.RemoveItemAfterRoundEnd = conVar.GetBool();
            UI.CvarChangeNotify(conVar.Name, Cvar.RemoveItemAfterRoundEnd.ToString(), conVar.Flags.HasFlag(ConVarFlags.Notify));
        }

        private void OnCvarChanged_ServerLanguage(IConVar conVar)
        {
            var value = conVar.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                Cvar.ServerLanguage = value.Replace("\"", "");
                UI.CvarChangeNotify(conVar.Name, Cvar.ServerLanguage, conVar.Flags.HasFlag(ConVarFlags.Notify));
            }
        }
    }

    static class Cvar
    {
        public static bool TeamOnly = true;
        public static short AdminChat = 0;
        public static short AdminHud = 0;
        public static short PlayerFormat = 3;

        public static bool BlockEPickup = true;
        public static float Delay = 1.0f;
        public static bool GlobalBlock = false;
        public static short DisplayAbility = 4;
        public static bool UsePriority = true;
        public static bool DisplayMapCommands = true;

        public static string SchemeName = "default.json";
        public static bool LowerMapname = false;

        public static bool GlowSpawn = false;
        public static bool GlowParticle = false;
        public static bool GlowProp = true;
        public static bool GlowVIP = false;

        public static int BanTime = 0;
        public static int BanLong = 720;
        public static string BanReason = "Trolling";
        public static string UnBanReason = "Giving another chance";
        public static bool KeepExpiredBan = true;
        public static int OfflineClearTime = 30;

        public static bool ClanTag = true;
        public static bool ClanTagInfo = true;

        public static bool RemoveItemAfterRoundEnd = true;

        public static string ServerLanguage = "en-us";
    }
}
