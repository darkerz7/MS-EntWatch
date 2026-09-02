using MS_EntWatch.Helpers;
using MS_EntWatch.Items;
using MS_EntWatch.Modules;
using MS_EntWatch.Modules.Eban;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.HookParams;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MS_EntWatch
{
    public partial class EntWatch
    {
        private static void OnPrecacheResources()
        {
            _modSharp!.PrecacheResource("particles/overhead_icon_fx/player_ping_ground_rings.vpcf");
        }

        private static void OnMapStart_Listener()
        {
            EW.CleanData();
            EW.LoadScheme();
            EW.LoadConfig();
            if (EW.g_Timer != null)
            {
                _modSharp!.StopTimer((Guid)EW.g_Timer);
                EW.g_Timer = null;
            }
            EW.g_Timer = _modSharp!.PushTimer(TimerUpdate, 1.0f, GameTimerFlags.Repeatable);

            Task.Run(() =>
            {
                LogManager.SystemAction("EntWatch.Info.ChangeMap", true, _modSharp!.GetMapName()!);
            });
            EW.GetorCreateHudLayout();
        }

        private static void OnMapEnd_Listener()
        {
            EW.RemoveHudLayout();
            EW.CleanData();
            if (EW.g_Timer != null)
            {
                _modSharp!.StopTimer((Guid)EW.g_Timer);
                EW.g_Timer = null;
            }
        }

        private static void OnEventRoundStart()
        {
            EW.g_ItemList.Clear();
            foreach (var client in _clients!.GetGameClients(true).ToArray())
            {
                if (client.GetPlayerController() is { } player) ClanTag.RemoveClanTag(player);
                if (EW.CheckDictionary(client))
                {
                    EW.g_EWPlayer[client].BannedPlayer.bBanTrigger = EW.g_EWPlayer[client].BannedPlayer.bBanned;
                    EW.g_EWPlayer[client].UsePriorityPlayer.UpdateCountButton(client);
                }
            }
        }

        private static void OnEventRoundEnd()
        {
            EW.g_ItemList.Clear();
            //Remove items after the end of the round
            if (Cvar.RemoveItemAfterRoundEnd)
            {
                foreach (var player in _entities!.FindPlayerControllers(true).ToList())
                {
                    if (player.GetPlayerPawn() is { } pawn && pawn.GetWeaponService() is { } service)
                    {
                        foreach (var weaponhandle in service.GetMyWeapons())
                        {
                            if (_entities!.FindEntityByHandle(weaponhandle) is { } weapon && !string.IsNullOrEmpty(weapon.HammerId)) weapon.Kill();
                        }
                    }
                }
            }
        }

        private static void OnEntitySpawned_Listener(IBaseEntity entity)
        {
            if (!EW.g_CfgLoaded) return;
            if (!entity.IsValid() || string.IsNullOrEmpty(entity.HammerId)) return;

            if (entity.Classname.Contains("weapon_"))
            {
                if (entity.AsBaseWeapon() is { } weapon) EW.WeaponIsItem(weapon);
            }

            else if (EW.HookOutput_CheckTrackedClasses(entity.Classname))
                EW.ButtonForItem(entity);

            else if (string.Equals(entity.Classname, "math_counter"))
                EW.CounterForItem(entity);
        }

        private static void OnEntityDeleted_Listener(IBaseEntity entity)
        {
            if (!EW.g_CfgLoaded) return;
            if (!entity.IsValid() || string.IsNullOrEmpty(entity.HammerId)) return;

            if (entity.Classname.Contains("weapon_"))
            {
                if (entity.AsBaseWeapon() is { } weapon)
                {
                    foreach (Item ItemTest in EW.g_ItemList.ToList())
                    {
                        if (ItemTest.WeaponHandle == weapon) EW.g_ItemList.Remove(ItemTest);
                    }
                }
            }

            else if (EW.HookOutput_CheckTrackedClasses(entity.Classname))
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                    {
                        if (AbilityTest.Entity == entity)
                        {
                            ItemTest.AbilityList.Remove(AbilityTest);
                            if (ItemTest.Owner != null && EW.CheckDictionary(ItemTest.Owner)) EW.g_EWPlayer[ItemTest.Owner].UsePriorityPlayer.UpdateCountButton(ItemTest.Owner);
                        }
                    }
                }
            }

            else if (string.Equals(entity.Classname, "math_counter"))
            {
                if (entity.As<IMathCounter>() is { } mathcounter)
                {
                    foreach (Item ItemTest in EW.g_ItemList.ToList())
                    {
                        foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                        {
                            if (!string.IsNullOrEmpty(AbilityTest.MathID) && !string.Equals(AbilityTest.MathID, "0") && AbilityTest.MathCounter == mathcounter)
                            {
                                ItemTest.AbilityList.Remove(AbilityTest);
                                if (ItemTest.Owner != null && EW.CheckDictionary(ItemTest.Owner)) EW.g_EWPlayer[ItemTest.Owner].UsePriorityPlayer.UpdateCountButton(ItemTest.Owner);
                            }
                        }
                    }
                }
            }
        }

        private void LoadClientPrefs(IGameClient client)
        {
            if (client == null || !client.IsValid || GetClientPrefs() is not { } cp || !cp.IsLoaded(client.SteamId)) return;

            if (client.GetPlayerController() is { } player && EW.CheckDictionary(client))
            {
                //HUD Show
                if (cp.GetCookie(client.SteamId, "EW_HUD_Show") is { } cookie_hud_show)
                {
                    string sValue = cookie_hud_show.GetString();
                    if (string.IsNullOrEmpty(sValue)) EW.g_EWPlayer[client].HudPlayer.bShow = true;
                    else EW.g_EWPlayer[client].HudPlayer.bShow = !string.Equals(sValue, "0");
                }
                else
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Show", "1");
                    EW.g_EWPlayer[client].HudPlayer.bShow = true;
                }
                //Size
                if (cp.GetCookie(client.SteamId, "EW_HUD_SizeType") is { } cookie_hud_size)
                {
                    string sValue = cookie_hud_size.GetString();
                    if (string.IsNullOrEmpty(sValue) || !byte.TryParse(sValue, out byte iValue)) iValue = 1;
                    EW.g_EWPlayer[client].HudPlayer.iSize = iValue;
                }
                else
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Size", "1");
                    EW.g_EWPlayer[client].HudPlayer.iSize = 1;
                }
                //Refresh
                if (cp.GetCookie(client.SteamId, "EW_HUD_Refresh") is { } cookie_hud_refresh)
                {
                    string sValue = cookie_hud_refresh.GetString();
                    if (string.IsNullOrEmpty(sValue) || !Int32.TryParse(sValue, out int iValue)) iValue = 3;
                    EW.g_EWPlayer[client].HudPlayer.iRefresh = iValue;
                }
                else
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Refresh", "3");
                    EW.g_EWPlayer[client].HudPlayer.iRefresh = 3;
                }
                //PlayerInfo Format
                if (cp.GetCookie(client.SteamId, "EW_PInfo_Format") is { } cookie_hud_pliformat)
                {
                    string sValue = cookie_hud_pliformat.GetString();
                    if (string.IsNullOrEmpty(sValue) || !Int32.TryParse(sValue, out int iValue)) iValue = Cvar.PlayerFormat;
                    EW.g_EWPlayer[client].PFormatPlayer = iValue;
                }
                else
                {
                    cp.SetCookie(client.SteamId, "EW_PInfo_Format", $"{Cvar.PlayerFormat}");
                    EW.g_EWPlayer[client].PFormatPlayer = Cvar.PlayerFormat;
                }
                //Use Priority
                if (cp.GetCookie(client.SteamId, "EW_Use_Priority") is { } cookie_hud_usepriority)
                {
                    string sValue = cookie_hud_usepriority.GetString();
                    if (string.IsNullOrEmpty(sValue)) EW.g_EWPlayer[client].UsePriorityPlayer.Activate = true;
                    else EW.g_EWPlayer[client].UsePriorityPlayer.Activate = !string.Equals(sValue, "0");
                }
                else
                {
                    cp.SetCookie(client.SteamId, "EW_Use_Priority", "1");
                    EW.g_EWPlayer[client].UsePriorityPlayer.Activate = true;
                }
            }
        }

        private static void OnEventPlayerConnectFull(IGameClient client)
        {
            OfflineFunc.PlayerConnectFull(client);

            if (client.IsValid)
            {
                EW.CheckDictionary(client); //Add EWPlayer
            }

           EbanPlayer.GetBan(client, true); //Set Eban
        }

        private static void OnEventPlayerDisconnect(IGameClient client)
        {
            OfflineFunc.PlayerDisconnect(client);

            EW.g_EWPlayer.Remove(client);   //Remove EWPlayer

            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (ItemTest.Owner == client)
                {
                    ItemTest.Owner = null;
                    if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Disconnect", EW.g_Scheme.Color_disconnect, ItemTest, client);
                    EW.g_cAPI.OnPlayerDisconnectWithItem(ItemTest.Name, client);
                    if (client.GetPlayerController() is { } player) ClanTag.RemoveClanTag(player);
                    ItemTest.EnableGlow();
                    if (!ItemTest.ForceDrop)
                    {
                        ItemTest.WeaponHandle.Kill();
                    }
                }
            }
            EW.DropSpecialWeapon(client);
        }

        private static void OnEventPlayerDeath(IPlayerKilledForwardParams @params)
        {
            if (!EW.g_CfgLoaded) return;
            
            if (@params.Client.IsValid)
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    if (ItemTest.Owner == @params.Client)
                    {
                        ItemTest.Owner = null;
                        if (EW.CheckDictionary(@params.Client)) EW.g_EWPlayer[@params.Client].UsePriorityPlayer.UpdateCountButton(@params.Client);
                        if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Death", EW.g_Scheme.Color_death, ItemTest, @params.Client);
                        EW.g_cAPI.OnPlayerDeathWithItem(ItemTest.Name, @params.Client);
                        ClanTag.RemoveClanTag(@params.Controller);
                        ItemTest.EnableGlow();
                        if (!ItemTest.ForceDrop)
                        {
                            ItemTest.WeaponHandle.Kill();
                        }
                    }
                }
                EW.DropSpecialWeapon(@params.Client);
            }
        }

        private static void OnEventPlayerPressUse(IPlayerRunCommandHookParams @params)
        {
            if (!EW.g_CfgLoaded || !Cvar.UsePriority) return;

            if (@params.Service.KeyChangedButtons.HasFlag(UserCommandButtons.Use)
                && @params.Service.KeyButtons.HasFlag(UserCommandButtons.Use))
            {
                if (!EW.CheckDictionary(@params.Client)) return;
                EW.g_EWPlayer[@params.Client].UsePriorityPlayer.DetectUse(@params.Client);
            }
        }

        private static void OnWeaponPickup(IPlayerEquipWeaponForwardParams @params)
        {
            if (!EW.g_CfgLoaded) return;
            if (@params.Client is { IsValid: true } client && client.GetPlayerController() is { } player)
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    if (ItemTest.ThisItem(@params.Weapon.Index))
                    {
                        ItemTest.Owner = client;
                        ItemTest.Team = player.Team;
                        ItemTest.SetDelay();
                        if (EW.CheckDictionary(client)) EW.g_EWPlayer[client].UsePriorityPlayer.UpdateCountButton(client);
                        if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Pickup", EW.g_Scheme.Color_pickup, ItemTest, ItemTest.Owner);
                        EW.g_cAPI.OnPickUpItem(ItemTest.Name, client);
                        ClanTag.UpdatePickUp(ItemTest);
                        ItemTest.DisableGlow();
                        foreach (OfflineBan OfflineTest in EW.g_OfflinePlayer.ToList())
                        {
                            if (client.UserId == OfflineTest.UserID)
                            {
                                OfflineTest.LastItem = ItemTest.Name;
                                break;
                            }
                        }
                        return;
                    }
                }
            }
        }

        private static void OnWeaponDrop(IPlayerDropWeaponForwardParams @params)
        {
            if (!EW.g_CfgLoaded) return;

            if (@params.Client is { IsValid: true } client && client.GetPlayerController() is { } player)
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    if (ItemTest.WeaponHandle == @params.Weapon)
                    {
                        if (ItemTest.Owner == client)
                        {
                            ItemTest.Owner = null;
                            if (EW.CheckDictionary(client)) EW.g_EWPlayer[client].UsePriorityPlayer.UpdateCountButton(client);
                            if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Drop", EW.g_Scheme.Color_drop, ItemTest, client);
                            EW.g_cAPI.OnDropItem(ItemTest.Name, client);
                            ClanTag.RemoveClanTag(player);
                            ItemTest.EnableGlow();
                        }
                        return;
                    }
                }
            }
        }

        private static HookReturnValue<bool> OnWeaponCanUse(IPlayerWeaponCanUseHookParams @params)
        {
            if (!EW.g_CfgLoaded) return default;

            if (@params.Client is { IsValid: true } client && @params.Weapon is { IsValidEntity:true } weapon)
            {
                if (EW.CheckDictionary(client) && EW.g_EWPlayer[client].BannedPlayer.bFixSpawnItem)
                {
                    return new HookReturnValue<bool>(EHookAction.SkipCallReturnOverride);
                }

                if (client.GetPlayerController() is { } player && player.GetPawn() is { } pawn && pawn.GetMovementService() is { } service)
                {
                    foreach (Item ItemTest in EW.g_ItemList.ToList())
                    {
                        if (ItemTest.WeaponHandle == weapon && (Cvar.BlockEPickup && (service.KeyButtons & UserCommandButtons.Use) != 0 || (EW.CheckDictionary(client) && EW.g_EWPlayer[client].BannedPlayer.bBanned)))
                        {
                            return new HookReturnValue<bool>(EHookAction.SkipCallReturnOverride);
                        }
                    }
                }
            }

            return default;
        }

        private static unsafe delegate* unmanaged<nint, nint, bool> CBaseTrigger_PassesTriggerFilters;
        private unsafe bool Hook_TriggersTouch()
        {
            var vFuncIndex = _modSharp!.GetGameData().GetVFuncIndex("CBaseTrigger::PassesTriggerFilters");
            _virtualHook.Prepare("server", "CTriggerOnce", vFuncIndex, (nint)(delegate* unmanaged<nint, nint, bool>)(&Hook_CBaseTrigger_PassesTriggerFilters));
            if (!_virtualHook.Install()) return false;
            _virtualHook.Prepare("server", "CTriggerMultiple", vFuncIndex, (nint)(delegate* unmanaged<nint, nint, bool>)(&Hook_CBaseTrigger_PassesTriggerFilters));
            if (!_virtualHook.Install()) return false;
            _virtualHook.Prepare("server", "CTriggerTeleport", vFuncIndex, (nint)(delegate* unmanaged<nint, nint, bool>)(&Hook_CBaseTrigger_PassesTriggerFilters));
            if (!_virtualHook.Install()) return false;
            CBaseTrigger_PassesTriggerFilters = (delegate* unmanaged<nint, nint, bool>)_virtualHook.Trampoline;

            return true;
        }
        [UnmanagedCallersOnly]
        private static unsafe bool Hook_CBaseTrigger_PassesTriggerFilters(nint @ent, nint @act)
        {
            if (_entities!.MakeEntityFromPointer<IBaseEntity>(@ent) is not { IsValidEntity: true } entity) return false;
            if (_entities!.MakeEntityFromPointer<IBaseEntity>(@act) is not { IsValidEntity: true } activator) return false;

            if (EW.g_ItemConfig != null && activator.AsPlayerPawn() is { } pawn && pawn.GetController() is { } player && player.GetGameClient() is { } client && EW.CheckDictionary(client) && EW.g_EWPlayer[client].BannedPlayer.bBanTrigger)
            {
                foreach (ItemConfig ItemTest in EW.g_ItemConfig.ToList())
                {
                    if (!string.IsNullOrEmpty(ItemTest.TriggerID) && !string.Equals(ItemTest.TriggerID, "0") && string.Equals(ItemTest.TriggerID, entity.HammerId)) return false;
                }
            }
            return CBaseTrigger_PassesTriggerFilters(@ent, @act);
        }

        private static EHookAction OnButtonPressed(IBaseEntity entity, IBaseEntity? activator)
        {
            if (!EW.g_CfgLoaded) return default;

            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                {
                    if (AbilityTest.Entity is { } button && button.IsValid() && entity == button)
                    {
                        if (!ItemTest.CheckDelay() || !AbilityTest.Ready()) return EHookAction.SkipCallReturnOverride;
                        if (activator is { IsPlayerPawn: true } actpawn && //activator PlayerPawn
                            !(ItemTest.Owner is { IsValid: true } owner && owner.GetPlayerController()?.GetPawn()?.Index == actpawn.Index) && //Doesn't owned
                            (entity.Classname is "func_button" or "func_rot_button" or "func_physbox" || entity.Classname.StartsWith("func_door"))) //Only buttons
                                return EHookAction.SkipCallReturnOverride;
                        
                        if (activator != null && activator.IsValid()) AbilityTest.SetFilter(activator);
                        AbilityTest.Used();
                        if (ItemTest.Owner is { IsValid: true })
                        {
                            if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Use", EW.g_Scheme.Color_use, ItemTest, ItemTest.Owner, AbilityTest);
                            EW.g_cAPI.OnUseItem(ItemTest.Name, ItemTest.Owner, AbilityTest.Name);
                        }
                        return default;
                    }
                }
            }
            return default;
        }

        private static EHookAction OnInput(IBaseEntity entity, string input, in EntityVariant value, IBaseEntity? activator)
        {
            if (!EW.g_CfgLoaded || activator == null || !activator.IsValid()) return default;

            if (entity.PrivateVScripts.Equals("game_ui", StringComparison.OrdinalIgnoreCase) && input.Equals("invalue", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                    {
                        if (AbilityTest.ButtonClass.StartsWith("game_ui::", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.Equals(AbilityTest.ButtonClass[9..], value.AsString, StringComparison.OrdinalIgnoreCase))
                            {
                                if (ItemTest.CheckDelay() && AbilityTest.Ready() && ItemTest.Owner is { IsValid: true } owner && owner.GetPlayerController()?.GetPawn()?.Index == activator.Index)
                                {
                                    AbilityTest.SetFilter(activator);
                                    AbilityTest.Used();
                                    if (EW.g_Scheme != null) UI.EWChatActivity("EntWatch.Chat.Use", EW.g_Scheme.Color_use, ItemTest, ItemTest.Owner, AbilityTest);
                                    EW.g_cAPI.OnUseItem(ItemTest.Name, ItemTest.Owner, AbilityTest.Name);
                                    return default;
                                }
                            }
                        }
                    }
                }
            }
            return default;
        }

        private static void OnTransmit()
        {
            if (!EW.g_CfgLoaded) return;

            if (Cvar.GlowVIP)
            {
                foreach (var ewp in EW.g_EWPlayer.ToList())
                {
                    if (!ewp.Value.PrivilegePlayer.WeaponGlow && ewp.Key.GetPlayerController() is { } player)
                    {
                        foreach (var ItemTest in EW.g_ItemList.ToList())
                        {
                            if (ItemTest.Particle is { IsValidEntity: true } particle && _transmits!.IsEntityHooked(particle)) _transmits!.SetEntityState(particle.Index, player.Index, false, -1);
                            if (ItemTest.Prop is { IsValidEntity: true } prop && _transmits!.IsEntityHooked(prop)) _transmits!.SetEntityState(prop.Index, player.Index, false, -1);
                        }
                    }
                }
            }
        }

        public static void TimerUpdate()
        {
            if (!EW.g_CfgLoaded) return;

            EW.ShowHud();
            ClanTag.UpdateClanTag();
        }

        public static void TimerRetry() => EbanDB.CheckConnection();

        public static void TimerUnban()
        {
            string? sServerName = EW.g_Scheme?.Server_name;
            if (string.IsNullOrEmpty(sServerName)) { sServerName = "Zombies Server"; }

            int iTime = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            EbanDB.OfflineUnban(sServerName, iTime);

            Task.Run(() =>
            {
                OfflineFunc.TimeToClear();
            });

            //Update (Un)Bans
            Task.Run(() =>
            {
                Parallel.ForEach(EW.g_EWPlayer, (pair) =>
                {
                    if (pair.Value.BannedPlayer.iDuration > 0 && pair.Value.BannedPlayer.iTimeStamp_Issued < iTime) pair.Value.BannedPlayer.bBanned = false;
                    EbanPlayer.GetBan(pair.Key);
                });
            });
        }
    }
}
