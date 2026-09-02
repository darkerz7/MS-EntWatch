using MS_EntWatch.Helpers;
using MS_EntWatch.Items;
using MS_EntWatch.Modules.Eban;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using System.Collections.Frozen;
using System.Globalization;
using System.Text.Json;

namespace MS_EntWatch
{
    static class EW
    {
        public static readonly byte HUDCHANNEL = 10;

        public static double fGameTime;
        public static List<ItemConfig>? g_ItemConfig = [];
        public static List<Item> g_ItemList = [];
        public static Scheme? g_Scheme = new();
        public static bool g_CfgLoaded = false;
        public static EWAPI g_cAPI = new();

        public static Guid? g_Timer = null;
        public static Guid? g_TimerRetryDB = null;
        public static Guid? g_TimerUnban = null;

        public static Dictionary<IGameClient, EWPlayer> g_EWPlayer = [];
        public static List<OfflineBan> g_OfflinePlayer = [];
        readonly static HashSet<(string, string)> g_HookedOutputs = [];
        readonly static HashSet<string> g_HookOutput_TrackedClasses = [];
        static ICustomHudLayout? g_CustomHudLayout = null;
        public static CultureInfo cultureEN = new("en-EN");

        public static void InitTimers()
        {
            InitTimerUpdater();
            /*foreach (var client in EntWatch._clients!.GetGameClients(true).ToArray())
            {
                CheckDictionary(client);
                OfflineFunc.PlayerConnectFull(client);
            }*/
            InitTimerRetry();
            InitTimerUnban();
        }

        public static void RemoveTimers()
        {
            if (g_Timer != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_Timer);
                g_Timer = null;
            }
            if (g_TimerRetryDB != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_TimerRetryDB);
                g_TimerRetryDB = null;
            }
            if (g_TimerUnban != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_TimerUnban);
                g_TimerUnban = null;
            }
        }

        public static void InitTimerUpdater()
        {
            if (g_Timer != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_Timer);
                g_Timer = null;
            }
            g_Timer = EntWatch._modSharp!.PushTimer(EntWatch.TimerUpdate, 1.0f, GameTimerFlags.Repeatable);
        }

        private static void InitTimerRetry()
        {
            if (g_TimerRetryDB != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_TimerRetryDB);
                g_TimerRetryDB = null;
            }
            g_TimerRetryDB = EntWatch._modSharp!.PushTimer(EntWatch.TimerRetry, 5.0f, GameTimerFlags.Repeatable);
        }

        private static void InitTimerUnban()
        {
            if (g_TimerUnban != null)
            {
                EntWatch._modSharp!.StopTimer((Guid)g_TimerUnban);
                g_TimerUnban = null;
            }
            g_TimerUnban = EntWatch._modSharp!.PushTimer(EntWatch.TimerUnban, 60.0f, GameTimerFlags.Repeatable);
        }

        public static bool CheckDictionary(IGameClient client)
        {
            if (!g_EWPlayer.ContainsKey(client))
                return g_EWPlayer.TryAdd(client, new EWPlayer());
            return true;
        }

        public static void UpdateTime()
        {
            fGameTime = EntWatch._modSharp!.EngineTime();
        }
        public static void CleanData()
        {
            g_CfgLoaded = false;
            g_ItemList.Clear();
            g_ItemConfig?.Clear();
        }

        public static void LoadConfig()
        {
            if (EntWatch._modSharp!.GetMapName() is { } mapname)
            {
                string sFileName = $"{EntWatch._dllPath!}/maps/{(Cvar.LowerMapname ? mapname.ToLower() : mapname)}.json";
                string sFileNameOverride = $"{sFileName[..^5]}_override.json";
                string sData;
                try
                {
                    if (File.Exists(sFileNameOverride))
                    {
                        sData = File.ReadAllText(sFileNameOverride);
                        UI.EWSysInfo("EntWatch.Info.Cfg.Loading", 7, sFileNameOverride);
                    }
                    else if (File.Exists(sFileName))
                    {
                        sData = File.ReadAllText(sFileName);
                        UI.EWSysInfo("EntWatch.Info.Cfg.Loading", 7, sFileName);
                    }
                    else
                    {
                        UI.EWSysInfo("EntWatch.Info.Cfg.NotFound", 14);
                        return;
                    }
                    g_ItemConfig = JsonSerializer.Deserialize<List<ItemConfig>>(sData);
                    g_CfgLoaded = true;
                }
                catch (Exception e)
                {
                    UI.EWSysInfo("EntWatch.Info.Error", 15, $"Bad Config file for {(Cvar.LowerMapname ? mapname.ToLower() : mapname)}!");
                    UI.EWSysInfo("EntWatch.Info.Error", 15, $"{e.Message}");
                    g_CfgLoaded = false;
                }
                if (g_CfgLoaded && g_ItemConfig is { })
                {
                    foreach (ItemConfig ItemTest in g_ItemConfig.ToList())
                        foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                            AddHookOutput(AbilityTest.ButtonClass, AbilityTest.Event);
                }
            }
        }

        public static void LoadScheme()
        {
            string sFileName = $"{EntWatch._dllPath!}/scheme/{Cvar.SchemeName}";
            string sData;
            try
            {
                if (File.Exists(sFileName) || File.Exists(sFileName = $"{EntWatch._dllPath!}/scheme/default.json"))
                {
                    sData = File.ReadAllText(sFileName);
                    UI.EWSysInfo("EntWatch.Info.Scheme.Loading", 7, sFileName);
                }
                else
                {
                    UI.EWSysInfo("EntWatch.Info.Scheme.NotFound", 14);
                    return;
                }
                g_Scheme = JsonSerializer.Deserialize<Scheme>(sData);
            }
            catch (Exception e)
            {
                UI.EWSysInfo("EntWatch.Info.Error", 15, $"Bad Scheme file for {Cvar.SchemeName}!");
                UI.EWSysInfo("EntWatch.Info.Error", 15, $"{e.Message}");
                g_CfgLoaded = false;
            }
        }

        public static bool WeaponIsItem(IBaseWeapon weapon)
        {
            if (g_ItemConfig != null)
            {
                foreach (ItemConfig ItemTest in g_ItemConfig.ToList())
                {
                    if (ItemTest.ThisItemConfig(weapon.HammerId))
                    {
                        Item cNewItem = new(ItemTest, weapon);
                        g_ItemList.Add(cNewItem);
                        //Auto block after adding to the list
                        if (cNewItem.BlockPickup || Cvar.GlobalBlock) cNewItem.WeaponHandle.CanBePickedUp = false;
                        else cNewItem.WeaponHandle.CanBePickedUp = true;
                        return true;
                    }
                }
            }
            
            return false;
        }

        public static void DropSpecialWeapon(IGameClient client)
        {
            if (client.IsValid && client.GetPlayerController() is { } player && player.IsValid() && player.GetPlayerPawn() is { } pawn && pawn.GetWeaponService() is { } service)
            {
                Vector vecDeath = pawn.GetCenter();
                foreach (var weaponhandle in service.GetMyWeapons())
                {
                    if (EntWatch._entities!.FindEntityByHandle(weaponhandle) is { } weapon && !string.IsNullOrEmpty(weapon.HammerId))
                    {
                        weapon.NextOwnerTouchTime = EntWatch._modSharp!.GetGlobals().CurTime + 0.1f;
                        pawn.DropWeapon(weapon);
                        EntWatch._modSharp!.PushTimer(() =>
                        {
                            if (weapon.IsValid()) weapon.SetAbsOrigin(vecDeath);
                        }, 0.05f);
                    }
                }
            }
        }

        public static bool ButtonForItem(IBaseEntity entity)
        {
            if (EntityParentRecursive(entity)?.AsBaseWeapon() is { } weapon && weapon.IsValid())
            {
                if(entity.Classname.StartsWith("func_door") && ((entity.SpawnFlags & 256) != 1)) return false;

                foreach (Item ItemTest in g_ItemList.ToList())
                {
                    if (weapon.Index == ItemTest.WeaponHandle.Index)
                    {
                        foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                        {
                            if (string.Equals(AbilityTest.ButtonID, entity.HammerId) || string.IsNullOrEmpty(AbilityTest.ButtonID) || string.Equals(AbilityTest.ButtonID, "0"))
                            {
                                AbilityTest.Entity = entity;
                                AbilityTest.ButtonID = entity.HammerId;
                                AbilityTest.ButtonClass = entity.Classname;
                                return true;
                            }
                        }
                        Ability abilitytest = new("", entity.Classname, true, 0, 0, 0, entity.HammerId, entity);
                        ItemTest.AbilityList.Add(abilitytest);
                        return true;
                    }
                }
            }
            return false;
        }

        public static IBaseEntity? EntityParentRecursive(IBaseEntity entity)
        {
            if (entity == null || !entity.IsValid()) return null;

            if (entity.Classname.Contains("weapon_")) return entity;

            if (entity.GetBodyComponent()?.GetSceneNode()?.GetParent()?.GetOwner() is { } Owner && Owner.IsValid() && !string.Equals(Owner.Name, "")) return EntityParentRecursive(Owner);
            
            return null;
        }

        public static void CounterForItem(IBaseEntity entity)
        {
            EntWatch._modSharp!.PushTimer(() =>
            {
                if (entity == null || !entity.IsValid() || string.IsNullOrEmpty(entity.Name)) return; //Bad math_counter
                if (entity.As<IMathCounter>() is { } MathCounter)
                {
                    foreach (Item ItemTest in g_ItemList.ToList())
                    {
                        if (ItemTest.WeaponHandle == null || !ItemTest.WeaponHandle.IsValid()) continue;
                        foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                        {
                            if (!AbilityTest.MathFindSpawned && !string.IsNullOrEmpty(AbilityTest.MathID) && !string.Equals(AbilityTest.MathID, "0") && string.Equals(AbilityTest.MathID, MathCounter.HammerId))
                            {
                                if (AbilityTest.MathNameFix) // <objectname> + _ + <serial number from 1> example: weapon_fire_125
                                {
                                    if (string.IsNullOrEmpty(ItemTest.WeaponHandle.Name)) continue;
                                    int iIndexWeapon = ItemTest.WeaponHandle.Name.LastIndexOf('_');
                                    if (iIndexWeapon == -1) continue;
                                    int iIndexMathCounter = MathCounter.Name.LastIndexOf('_');
                                    if (iIndexMathCounter == -1) return; //Another math_counter or bad EW config
                                    if (!string.Equals(MathCounter.Name[iIndexMathCounter..], ItemTest.WeaponHandle.Name[iIndexWeapon..])) continue;
                                }
                                AbilityTest.MathCounter = MathCounter;
                                return;
                            }
                        }

                    }
                }
            }, 2.0f, GameTimerFlags.StopOnRoundEnd);
        }

        public static void ShowHud()
        {
            UpdateTime();

            foreach (var client in EntWatch._clients!.GetGameClients(true).ToArray())
            {
                if (CheckDictionary(client) && g_EWPlayer[client].HudPlayer != null && client.GetPlayerController() is { } player) g_EWPlayer[client].HudPlayer.UpdateHUD(player);
            }
        }

        public static void AddHookOutput(string classname, string output)
        {
            if (string.IsNullOrEmpty(output) || string.IsNullOrEmpty(classname)) return;

            string sClassname_Lower = classname.ToLower();

            if (HookOutput_ValidClassList.Contains(sClassname_Lower))
            {
                string sOutput_Lower = output.ToLower();
                if (g_HookedOutputs.Add((sClassname_Lower, sOutput_Lower)))
                {
                    EntWatch._entities!.HookEntityOutput(sClassname_Lower, sOutput_Lower);
                    g_HookOutput_TrackedClasses.Add(sClassname_Lower);
                }
            }
        }

        public static bool HookOutput_CheckTrackedClasses(string classname)
        {
            if (string.IsNullOrEmpty(classname)) return false;

            return g_HookOutput_TrackedClasses.Contains(classname.ToLower());
        }

        static readonly FrozenSet<string> HookOutput_ValidClassList = [
            "func_button",
            "func_physbox",
            "func_door",
            "func_rot_button",
            "func_door_rotating",
            "logic_case",
            "logic_relay",
            "logic_timer",
            "logic_branch_listener",
            "logic_branch",
            "logic_compare",
            "math_counter",
            "trigger_gravity",
            "trigger_hurt",
            "trigger_look",
            "trigger_multiple",
            "trigger_once",
            "trigger_push",
            "trigger_teleport",
            "trigger_wind",
            "env_entity_maker",
            "point_template",
            "filter_activator_attribute_int",
            "filter_activator_class",
            "filter_activator_context",
            "filter_activator_model",
            "filter_activator_name",
            "filter_multi",
            "filter_activator_team",
            "func_breakable"
            ];

        public static ICustomHudLayout? GetorCreateHudLayout()
        {
            if (g_CustomHudLayout != null) return g_CustomHudLayout;
            return g_CustomHudLayout = EntWatch._panorama!.CreateLayout("panorama/layout/custom_game/entwatch_dz.vxml_c");
        }

        public static void RemoveHudLayout()
        {
            g_CustomHudLayout = null;
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

        public static float Distance(Vector point1, Vector point2)
        {
            float dx = point2.X - point1.X;
            float dy = point2.Y - point1.Y;
            float dz = point2.Z - point1.Z;

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
