using MS_EntWatch.Items;
using MS_EntWatch.Modules.Eban;
using MS_EntWatch_Shared;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;

namespace MS_EntWatch
{
    internal class EWAPI: IEntWatchAPI
    {
        public void Native_EntWatch_IsClientBanned(string sSteamID)
        {
            if (!string.IsNullOrEmpty(sSteamID)) EbanPlayer.GetBan(sSteamID, GetBanAPI_Handler);
        }

        readonly EbanDB.GetBanAPIFunc GetBanAPI_Handler = (sClientSteamID, DBQuery_Result) =>
        {
            if (DBQuery_Result is { } result && result.Count > 0 && result[0] is { } r0)
            {
                SEWAPI_Ban target = new()
                {
                    bBanned = true,
                    sAdminName = r0[0] ?? "Console",
                    sAdminSteamID = r0[1] ?? "Server",
                    iDuration = Convert.ToInt32(r0[2] ?? $"{Cvar.BanTime}"),
                    iTimeStamp_Issued = Convert.ToInt32(r0[3] ?? $"{Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds())}"),
                    sReason = r0[4] ?? Cvar.BanReason,
                    sClientName = r0[5] ?? "",
                    sClientSteamID = sClientSteamID
                };
                EW.g_cAPI.IsClientBannedResult(target);
                return;
            }
            EW.g_cAPI.IsClientBannedResult(new SEWAPI_Ban());
        };

        public void Native_EntWatch_BanClient(SEWAPI_Ban sewPlayer)
        {
            if (EW.g_Scheme == null) return;
            EbanDB.BanClient(sewPlayer.sClientName, sewPlayer.sClientSteamID, sewPlayer.sAdminName, sewPlayer.sAdminSteamID, EW.g_Scheme.Server_name, sewPlayer.iDuration, sewPlayer.iTimeStamp_Issued, sewPlayer.sReason);
        }
        public void Native_EntWatch_UnbanClient(SEWAPI_Ban sewPlayer)
        {
            if (EW.g_Scheme == null) return;
            EbanDB.UnBanClient(sewPlayer.sClientSteamID, sewPlayer.sAdminName, sewPlayer.sAdminSteamID, EW.g_Scheme.Server_name, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), sewPlayer.sReason);
        }
        public void Native_EntWatch_UpdateStatusBanClient(IGameClient Player)
        {
            EbanPlayer.GetBan(Player);
        }
        public bool Native_EntWatch_IsSpecialItem(IBaseWeapon cEntity)
        {
            if (!cEntity.IsValid()) return false;
            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (ItemTest.WeaponHandle == cEntity) return true;
            }
            return false;
        }
        public bool Native_EntWatch_IsButtonSpecialItem(IBaseEntity cEntity)
        {
            if (!cEntity.IsValid()) return false;
            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                {
                    if (AbilityTest.Entity == cEntity) return true;
                }
            }
            return false;
        }
        public bool Native_EntWatch_HasSpecialItem(IGameClient player)
        {
            if (!player.IsValid) return false;
            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (ItemTest.Owner == player) return true;
            }
            return false;
        }
        public void Native_EntWatch_EnableWeaponGlow(IGameClient player)
        {
            if (player.IsValid && EW.CheckDictionary(player)) EW.g_EWPlayer[player].PrivilegePlayer.WeaponGlow = true;
        }
        public void Native_EntWatch_DisableWeaponGlow(IGameClient player)
        {
            if (player.IsValid && EW.CheckDictionary(player)) EW.g_EWPlayer[player].PrivilegePlayer.WeaponGlow = false;
        }
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnClientBanned? Forward_EntWatch_OnClientBanned;
        public void OnClientBanned(SEWAPI_Ban sewPlayer) => Forward_EntWatch_OnClientBanned?.Invoke(sewPlayer);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnClientUnbanned? Forward_EntWatch_OnClientUnbanned;
        public void OnClientUnbanned(SEWAPI_Ban sewPlayer) => Forward_EntWatch_OnClientUnbanned?.Invoke(sewPlayer);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnUseItem? Forward_EntWatch_OnUseItem;
        public void OnUseItem(string sItemName, IGameClient Player, string sAbility) => Forward_EntWatch_OnUseItem?.Invoke(sItemName, Player, sAbility);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnPickUpItem? Forward_EntWatch_OnPickUpItem;
        public void OnPickUpItem(string sItemName, IGameClient Player) => Forward_EntWatch_OnPickUpItem?.Invoke(sItemName, Player);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnDropItem? Forward_EntWatch_OnDropItem;
        public void OnDropItem(string sItemName, IGameClient Player) => Forward_EntWatch_OnDropItem?.Invoke(sItemName, Player);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnPlayerDisconnectWithItem? Forward_EntWatch_OnPlayerDisconnectWithItem;
        public void OnPlayerDisconnectWithItem(string sItemName, IGameClient Player) => Forward_EntWatch_OnPlayerDisconnectWithItem?.Invoke(sItemName, Player);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnPlayerDeathWithItem? Forward_EntWatch_OnPlayerDeathWithItem;
        public void OnPlayerDeathWithItem(string sItemName, IGameClient Player) => Forward_EntWatch_OnPlayerDeathWithItem?.Invoke(sItemName, Player);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnAdminSpawnItem? Forward_EntWatch_OnAdminSpawnItem;
        public void OnAdminSpawnItem(IGameClient Admin, string sItemName, IGameClient Target) => Forward_EntWatch_OnAdminSpawnItem?.Invoke(Admin, sItemName, Target);
        //===================================================================================================
        public event IEntWatchAPI.Forward_OnAdminTransferedItem? Forward_EntWatch_OnAdminTransferedItem;
        public void OnAdminTransferedItem(IGameClient Admin, string sItemName, IGameClient Receiver) => Forward_EntWatch_OnAdminTransferedItem?.Invoke(Admin, sItemName, Receiver);
        //===================================================================================================
        public event IEntWatchAPI.Forward_IsClientBannedResult? Forward_EntWatch_IsClientBannedResult;
        public void IsClientBannedResult(SEWAPI_Ban sewPlayer) => Forward_EntWatch_IsClientBannedResult?.Invoke(sewPlayer);
        //===================================================================================================
    }
}
