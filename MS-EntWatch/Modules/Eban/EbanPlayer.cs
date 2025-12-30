using MS_EntWatch.Helpers;
using MS_EntWatch_Shared;
using Sharp.Shared.Objects;

namespace MS_EntWatch.Modules.Eban
{
    internal class EbanPlayer
    {
        public bool bBanned;

        public string sAdminName;
        public string sAdminSteamID;
        public int iDuration;
        public int iTimeStamp_Issued;
        public string sReason;

        public string sClientName;
        public string sClientSteamID;

        public bool bBanTrigger;
        public bool bFixSpawnItem;

        public bool SetBan(string sBanAdminName, string sBanAdminSteamID, string sBanClientName, string sBanClientSteamID, int iBanDuration, string sBanReason)
        {
            if (!string.IsNullOrEmpty(sBanClientSteamID))
            {
                bBanned = true;
                bBanTrigger = true;
                sAdminName = sBanAdminName;
                sAdminSteamID = sBanAdminSteamID;
                sReason = sBanReason;
                if (iBanDuration < -1)
                {
                    iDuration = -1;
                    iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    UI.EWSysInfo("EntWatch.Info.Eban.Success", 6);
                }
                else if (iBanDuration == 0)
                {
                    iDuration = 0;
                    iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                }
                else
                {
                    iDuration = iBanDuration;
                    iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) + iDuration * 60;
                }
                SEWAPI_Ban apiBan = new()
                {
                    bBanned = bBanned,
                    sAdminName = sAdminName,
                    sAdminSteamID = sAdminSteamID,
                    iDuration = iDuration,
                    iTimeStamp_Issued = iTimeStamp_Issued,
                    sReason = sReason,
                    sClientName = sClientName,
                    sClientSteamID = sClientSteamID
                };
                EW.g_cAPI.OnClientBanned(apiBan);
                if (EW.g_Scheme != null) EbanDB.BanClient(sBanClientName, sBanClientSteamID, sAdminName, sAdminSteamID, EW.g_Scheme.Server_name, iDuration, iTimeStamp_Issued, sReason);
                return true;
            }
            return false;
        }

        public bool UnBan(string sUnBanAdminName, string sUnBanAdminSteamID, string sUnBanClientSteamID, string sUnbanReason)
        {
            if (!string.IsNullOrEmpty(sUnBanClientSteamID))
            {
                bBanned = false;
                if (string.IsNullOrEmpty(sUnbanReason)) sUnbanReason = "Amnesty";
                SEWAPI_Ban apiBan = new()
                {
                    bBanned = bBanned,
                    sAdminName = sUnBanAdminName,
                    sAdminSteamID = sUnBanAdminSteamID,
                    iDuration = 0,
                    iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                    sReason = sUnbanReason,
                    sClientName = "",
                    sClientSteamID = sUnBanClientSteamID
                };
                EW.g_cAPI.OnClientUnbanned(apiBan);
                if (EW.g_Scheme != null) EbanDB.UnBanClient(sUnBanClientSteamID, sUnBanAdminName, sUnBanAdminSteamID, EW.g_Scheme.Server_name, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), sUnbanReason);
                return true;
            }
            return false;
        }

        public static void GetBan(IGameClient player, bool bShow = false)
        {
            if (EW.g_Scheme != null) EbanDB.GetBan(player, EW.g_Scheme.Server_name, GetBanPlayer_Handler, bShow);
        }

        static readonly EbanDB.GetBanPlayerFunc GetBanPlayer_Handler = (player, DBQuery_Result, bShow) =>
        {
            if (player.IsValid && EW.CheckDictionary(player))
            {
                if (DBQuery_Result is { } result && result.Count > 0 && result[0] is { } r0)
                {
                    EW.g_EWPlayer[player].BannedPlayer.bBanned = true;
                    EW.g_EWPlayer[player].BannedPlayer.bBanTrigger = true;
                    EW.g_EWPlayer[player].BannedPlayer.sAdminName = r0[0] ?? "Console";
                    EW.g_EWPlayer[player].BannedPlayer.sAdminSteamID = r0[1] ?? "Server";
                    EW.g_EWPlayer[player].BannedPlayer.iDuration = Convert.ToInt32(r0[2] ?? $"{Cvar.BanTime}");
                    EW.g_EWPlayer[player].BannedPlayer.iTimeStamp_Issued = Convert.ToInt32(r0[3] ?? $"{Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds())}");
                    EW.g_EWPlayer[player].BannedPlayer.sReason = r0[4] ?? Cvar.BanReason;
                    if (bShow)
                    {
                        UI.EWSysInfo("EntWatch.Info.Eban.PlayerConnect", 4, UI.ReplaceColorTags(UI.PlayerInfoFormat(player)[3], false), EW.g_EWPlayer[player].BannedPlayer.iDuration, EW.g_EWPlayer[player].BannedPlayer.iTimeStamp_Issued, UI.ReplaceColorTags(UI.PlayerInfoFormat(EW.g_EWPlayer[player].BannedPlayer.sAdminName, EW.g_EWPlayer[player].BannedPlayer.sAdminSteamID)[3], false), EW.g_EWPlayer[player].BannedPlayer.sReason);
                    }
                }
                else
                {
                    EW.g_EWPlayer[player].BannedPlayer.bBanned = false;
                }
            }
        };
        public static void GetBan(string sClientSteamID, IGameClient admin, string reason, bool bConsole, EbanDB.GetBanCommFunc handler)
        {
            if (EW.g_Scheme != null) EbanDB.GetBan(sClientSteamID, EW.g_Scheme.Server_name, admin, reason, bConsole, handler);
        }
        public static void GetBan(string sClientSteamID, EbanDB.GetBanAPIFunc handler)
        {
            if (EW.g_Scheme != null) EbanDB.GetBan(sClientSteamID, EW.g_Scheme.Server_name, handler);
        }

        public EbanPlayer()
        {
            bBanned = false;
            bBanTrigger = false;
            bFixSpawnItem = false;
            sAdminName = "";
            sAdminSteamID = "";
            sClientName = "";
            sClientSteamID = "";
            sReason = "";
        }
    }
}
