using Microsoft.Extensions.Configuration;
using MS_EntWatch_Shared;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace MS_EntWatch_Example
{
    public class EntWatchExampleAPI : IModSharpModule
    {
        public string DisplayName => "EntWatchAPI-Example";
        public string DisplayAuthor => "DarkerZ[RUS]";
#pragma warning disable IDE0060, IDE0290
        public EntWatchExampleAPI(ISharedSystem sharedSystem, string dllPath, string sharpPath, Version version, IConfiguration coreConfiguration, bool hotReload)
#pragma warning restore IDE0060, IDE0290
        {
            _modules = sharedSystem.GetSharpModuleManager();
            _clientmanager = sharedSystem.GetClientManager();
            _entities = sharedSystem.GetEntityManager();
        }

        private readonly ISharpModuleManager _modules;
        private readonly IClientManager _clientmanager;
        private readonly IEntityManager _entities;

        public bool Init()
        {
            _clientmanager.InstallCommandCallback("ewtest1", OnCommandTest1);
            _clientmanager.InstallCommandCallback("ewtest2", OnCommandTest2);
            _clientmanager.InstallCommandCallback("ewtest3", OnCommandTest3);
            _clientmanager.InstallCommandCallback("ewtest4", OnCommandTest4);
            _clientmanager.InstallCommandCallback("ewtest5", OnCommandTest5);
            _clientmanager.InstallCommandCallback("ewtest6", OnCommandTest6);
            _clientmanager.InstallCommandCallback("ewtest7", OnCommandTest7);
            _clientmanager.InstallCommandCallback("ewtest8", OnCommandTest8);
            _clientmanager.InstallCommandCallback("ewtest9", OnCommandTest9);
            return true;
        }

        public void Shutdown()
        {
            _clientmanager.RemoveCommandCallback("ewtest1", OnCommandTest1);
            _clientmanager.RemoveCommandCallback("ewtest2", OnCommandTest2);
            _clientmanager.RemoveCommandCallback("ewtest3", OnCommandTest3);
            _clientmanager.RemoveCommandCallback("ewtest4", OnCommandTest4);
            _clientmanager.RemoveCommandCallback("ewtest5", OnCommandTest5);
            _clientmanager.RemoveCommandCallback("ewtest6", OnCommandTest6);
            _clientmanager.RemoveCommandCallback("ewtest7", OnCommandTest7);
            _clientmanager.RemoveCommandCallback("ewtest8", OnCommandTest8);
            _clientmanager.RemoveCommandCallback("ewtest9", OnCommandTest9);
        }

        void DisplayBan(SEWAPI_Ban sewPlayer) { PrintToConsole($"Player {sewPlayer.sClientName} was banned {sewPlayer.sAdminName}"); }
        void DisplayUnBan(SEWAPI_Ban sewPlayer) { PrintToConsole($"Player {sewPlayer.sClientName} was unbanned {sewPlayer.sAdminName}"); }
        void DisplayUseItem(string sItemName, IGameClient Player, string sAbility) { PrintToConsole($"Player {Player.Name} used {sItemName}({sAbility})"); }
        void DisplayPickUpItem(string sItemName, IGameClient Player) { PrintToConsole($"Player {Player.Name} pickup {sItemName}"); }
        void DisplayDropItem(string sItemName, IGameClient Player) { PrintToConsole($"Player {Player.Name} dropped {sItemName}"); }
        void DisplayDisconnectWithItem(string sItemName, IGameClient Player) { PrintToConsole($"Player {Player.Name} disconnected with {sItemName}"); }
        void DisplayDeathWithItem(string sItemName, IGameClient Player) { PrintToConsole($"Player {Player.Name} death with {sItemName}"); }
        void DisplayAdminSpawnItem(IGameClient Admin, string sItemName, IGameClient Target) { PrintToConsole($"Admin {Admin.Name} spawned {sItemName} for {Target.Name}"); }
        void DisplayAdminTransferedItem(IGameClient Admin, string sItemName, IGameClient Receiver) { PrintToConsole($"Admin {Admin.Name} transfered {sItemName} for {Receiver.Name}"); }
        void DisplayClientBannedResult(SEWAPI_Ban sewPlayer) { if (sewPlayer.bBanned) PrintToConsole($"You {sewPlayer.sClientName}({sewPlayer.sClientSteamID}) have a eban. Duration: {sewPlayer.iDuration}"); else PrintToConsole($"You have NOT a eban"); }

        private ECommandAction OnCommandTest1(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                string? sSteamID = ConvertSteamID64ToSteamID(client.SteamId.ToString());
                if (string.IsNullOrEmpty(sSteamID))
                {
                    PrintToConsole("Failed SteamID");
                    return ECommandAction.Stopped;
                }
                _api.Native_EntWatch_IsClientBanned(sSteamID);
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest2(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                string? sSteamID = ConvertSteamID64ToSteamID(client.SteamId.ToString());
                if (string.IsNullOrEmpty(sSteamID))
                {
                    PrintToConsole("Failed SteamID");
                    return ECommandAction.Stopped;
                }
                SEWAPI_Ban ban = new()
                {
                    sAdminName = "Api",
                    sAdminSteamID = "SERVER",
                    iDuration = 5,
                    sReason = "Test Api Ban",
                    sClientName = client.Name,
                    sClientSteamID = sSteamID
                };
                ban.iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) + ban.iDuration * 60;
                _api.Native_EntWatch_BanClient(ban);
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest3(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                string? sSteamID = ConvertSteamID64ToSteamID(client.SteamId.ToString());
                if (string.IsNullOrEmpty(sSteamID))
                {
                    PrintToConsole("Failed SteamID");
                    return ECommandAction.Stopped;
                }
                SEWAPI_Ban ban = new()
                {
                    sAdminName = "Api",
                    sAdminSteamID = "SERVER",
                    sReason = "Test Api UnBan",
                    sClientName = client.Name,
                    sClientSteamID = sSteamID
                };
                ban.iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) + ban.iDuration * 60;
                _api.Native_EntWatch_UnbanClient(ban);
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest4(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                _api.Native_EntWatch_UpdateStatusBanClient(client);
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest5(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                if (command.ArgCount >= 1)
                {
                    if (!Int32.TryParse(command.GetArg(1), out int number)) number = 0;
                    IBaseWeapon? weapon = _entities.FindEntityByIndex(number)?.AsBaseWeapon();
                    if (weapon is { })
                    {
                        if (_api.Native_EntWatch_IsSpecialItem(weapon)) PrintToConsole("Entity is Special Item");
                        else PrintToConsole("Entity is NOT Special Item");
                        return ECommandAction.Stopped;
                    }
                }
                PrintToConsole("Error on number of entity");

            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest6(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                if (command.ArgCount >= 1)
                {
                    if (!Int32.TryParse(command.GetArg(1), out int number)) number = 0;
                    IBaseEntity? ent = _entities.FindEntityByIndex(number);
                    if (ent is { IsValidEntity: true } entity)
                    {
                        if (_api.Native_EntWatch_IsButtonSpecialItem(entity)) PrintToConsole("Entity is a Special Item Button");
                        else PrintToConsole("Entity is NOT a Special Item Button");
                        return ECommandAction.Stopped;
                    }
                }
                PrintToConsole("Error on number of entity");
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest7(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                if (_api.Native_EntWatch_HasSpecialItem(client)) PrintToConsole("You have a Special Item");
                else PrintToConsole("You have NOT a Special Item");
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest8(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                _api.Native_EntWatch_EnableWeaponGlow(client);
                PrintToConsole("Glow of special objects is allowed for you");
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnCommandTest9(IGameClient client, StringCommand command)
        {
            if (client.IsValid && GetEntWatch() is { } _api)
            {
                _api.Native_EntWatch_DisableWeaponGlow(client);
                PrintToConsole("Glow of special objects is prohibited for you");
            }

            return ECommandAction.Stopped;
        }

        public static void PrintToConsole(string sMessage)
        {
            Console.ForegroundColor = (ConsoleColor)8;
            Console.Write("[");
            Console.ForegroundColor = (ConsoleColor)6;
            Console.Write("EntWatch:TestAPI");
            Console.ForegroundColor = (ConsoleColor)8;
            Console.Write("] ");
            Console.ForegroundColor = (ConsoleColor)13;
            Console.WriteLine(sMessage);
            Console.ResetColor();
        }
        public static string? ConvertSteamID64ToSteamID(string steamId64)
        {
            if (ulong.TryParse(steamId64, out var communityId) && communityId > 76561197960265728)
            {
                var authServer = (communityId - 76561197960265728) % 2;
                var authId = (communityId - 76561197960265728 - authServer) / 2;
                return $"STEAM_0:{authServer}:{authId}";
            }
            return null;
        }

        //Init IEntWatchAPI
        public void OnAllModulesLoaded() => GetEntWatch();
        public void OnLibraryConnected(string name)
        {
            if (name.Equals("EntWatch")) GetEntWatch();
        }
        public void OnLibraryDisconnect(string name)
        {
            if (name.Equals("EntWatch"))
            {
                if (_ientwatch?.Instance is { } _api)
                {
                    _api.Forward_EntWatch_OnClientBanned -= DisplayBan;
                    _api.Forward_EntWatch_OnClientUnbanned -= DisplayUnBan;
                    _api.Forward_EntWatch_OnUseItem -= DisplayUseItem;
                    _api.Forward_EntWatch_OnPickUpItem -= DisplayPickUpItem;
                    _api.Forward_EntWatch_OnDropItem -= DisplayDropItem;
                    _api.Forward_EntWatch_OnPlayerDisconnectWithItem -= DisplayDisconnectWithItem;
                    _api.Forward_EntWatch_OnPlayerDeathWithItem -= DisplayDeathWithItem;
                    _api.Forward_EntWatch_OnAdminSpawnItem -= DisplayAdminSpawnItem;
                    _api.Forward_EntWatch_OnAdminTransferedItem -= DisplayAdminTransferedItem;
                    _api.Forward_EntWatch_IsClientBannedResult -= DisplayClientBannedResult;
                }
                _ientwatch = null;
            }
        }
        private IModSharpModuleInterface<IEntWatchAPI>? _ientwatch;
        private IEntWatchAPI? GetEntWatch()
        {
            if (_ientwatch?.Instance is null)
            {
                _ientwatch = _modules.GetOptionalSharpModuleInterface<IEntWatchAPI>(IEntWatchAPI.Identity);
                if (_ientwatch?.Instance is { } _api)
                {
                    _api.Forward_EntWatch_OnClientBanned += DisplayBan;
                    _api.Forward_EntWatch_OnClientUnbanned += DisplayUnBan;
                    _api.Forward_EntWatch_OnUseItem += DisplayUseItem;
                    _api.Forward_EntWatch_OnPickUpItem += DisplayPickUpItem;
                    _api.Forward_EntWatch_OnDropItem += DisplayDropItem;
                    _api.Forward_EntWatch_OnPlayerDisconnectWithItem += DisplayDisconnectWithItem;
                    _api.Forward_EntWatch_OnPlayerDeathWithItem += DisplayDeathWithItem;
                    _api.Forward_EntWatch_OnAdminSpawnItem += DisplayAdminSpawnItem;
                    _api.Forward_EntWatch_OnAdminTransferedItem += DisplayAdminTransferedItem;
                    _api.Forward_EntWatch_IsClientBannedResult += DisplayClientBannedResult;
                }
            }
            return _ientwatch?.Instance;
        }
    }
}
