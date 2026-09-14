using MS_EntWatch.Helpers;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace MS_EntWatch
{
    public partial class EntWatch
    {
        void RegCommands()
        {
            _clients!.InstallCommandCallback("hud", OnEWChangeHud);
            _clients!.InstallCommandCallback("hudcap", OnEWHudCapture);
            _clients!.InstallCommandCallback("hudsize", OnEWChangeHudSize);
            _clients!.InstallCommandCallback("hudpos", OnEWChangeHudPos);
            _clients!.InstallCommandCallback("hudrefresh", OnEWChangeHudRefresh);
            _clients!.InstallCommandCallback("epf", OnEWChangePlayerFormat);
            _clients!.InstallCommandCallback("eup", OnEWChangeUsePriority);
            _clients!.InstallCommandCallback("estatus", OnEWStatus);
        }

        void UnRegCommands()
        {
            _clients!.RemoveCommandCallback("hud", OnEWChangeHud);
            _clients!.RemoveCommandCallback("hudcap", OnEWHudCapture);
            _clients!.RemoveCommandCallback("hudsize", OnEWChangeHudSize);
            _clients!.RemoveCommandCallback("hudpos", OnEWChangeHudPos);
            _clients!.RemoveCommandCallback("hudrefresh", OnEWChangeHudRefresh);
            _clients!.RemoveCommandCallback("epf", OnEWChangePlayerFormat);
            _clients!.RemoveCommandCallback("eup", OnEWChangeUsePriority);
            _clients!.RemoveCommandCallback("estatus", OnEWStatus);
        }

        private ECommandAction OnEWChangeHud(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }

            bool bNewValue = EW.g_EWPlayer[client].HudPlayer.bShow;

            string? sValue = command.ArgCount > 0 ? command.GetArg(1) : null;
            if (!string.IsNullOrEmpty(sValue))
            {
                bNewValue = sValue.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sValue, "1");
            }
            else bNewValue = !bNewValue;

            EW.g_EWPlayer[client].HudPlayer.bShow = bNewValue;

            if (bNewValue)
            {
                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Show", "1");
                }
                UI.ReplyToCommand(client, "EntWatch.Reply.Hud.Enabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "");
            }
            else
            {
                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Show", "0");
                }
                UI.ReplyToCommand(client, "EntWatch.Reply.Hud.Disabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_disabled : "");
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWHudCapture(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }

            bool bNewValue = EW.g_EWPlayer[client].HudPlayer.bCaptureEnabled;

            string? sValue = command.ArgCount > 0 ? command.GetArg(1) : null;
            if (!string.IsNullOrEmpty(sValue))
            {
                bNewValue = sValue.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sValue, "1");
            }
            else bNewValue = !bNewValue;

            EW.g_EWPlayer[client].HudPlayer.CaptureChange(client.Slot, bNewValue);

            if (bNewValue) UI.ReplyToCommand(client, "EntWatch.Reply.Capture.Enabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "");
            else UI.ReplyToCommand(client, "EntWatch.Reply.Capture.Disabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_disabled : "");

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWChangeHudSize(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            int iArgNeed = 1;
            string sArgHelper = "[size] (default: 1; min 0(small); max 3(large))";
            if (command.ArgCount < iArgNeed)
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.MinArg", command.ChatTrigger, iArgNeed, command.CommandName, sArgHelper);
                return ECommandAction.Stopped;
            }

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }

            if (!byte.TryParse(command.GetArg(1), out byte number)) number = 1;
            if (number >= 0 && number <= 3)
            {
                EW.g_EWPlayer[client].HudPlayer.iSize = number;
                    
                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_SizeType", number.ToString());
                }

                UI.ReplyToCommand(client, "EntWatch.Reply.Hud.Size", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "", number);
            }
            else UI.ReplyToCommand(client, "EntWatch.Reply.NotValid", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "");

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWChangeHudPos(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            int iArgNeed = 1;
            string sArgHelper = "[position] (default: 0; min 0(middle); max 4(top))";
            if (command.ArgCount < iArgNeed)
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.MinArg", command.ChatTrigger, iArgNeed, command.CommandName, sArgHelper);
                return ECommandAction.Stopped;
            }

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }
            
            if (!byte.TryParse(command.GetArg(1), out byte number)) number = 0;
            if (number >= 0 && number <= 4)
            {
                EW.g_EWPlayer[client].HudPlayer.ChangePosition(client.Slot, number);

                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_PosNum", number.ToString());
                }

                UI.ReplyToCommand(client, "EntWatch.Reply.Hud.PositionNum", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "", number);
            }
            else UI.ReplyToCommand(client, "EntWatch.Reply.NotValid", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "");

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWChangeHudRefresh(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            int iArgNeed = 1;
            string sArgHelper = "[sec] (default: 3; min 1; max 10)";
            if (command.ArgCount < iArgNeed)
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.MinArg", command.ChatTrigger, iArgNeed, command.CommandName, sArgHelper);
                return ECommandAction.Stopped;
            }

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }
            if (!Int32.TryParse(command.GetArg(1), out int number)) number = 0;
            if (number >= 1 && number <= 10)
            {
                EW.g_EWPlayer[client].HudPlayer.iRefresh = number;

                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_HUD_Refresh", number.ToString());
                }

                UI.ReplyToCommand(client, "EntWatch.Reply.Hud.Refresh", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "", number);
            }
            else UI.ReplyToCommand(client, "EntWatch.Reply.NotValid", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "");

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWChangePlayerFormat(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            int iArgNeed = 1;
            string sArgHelper = "[number] (default: 3; min 0; max 3)";
            if (command.ArgCount < iArgNeed)
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.MinArg", command.ChatTrigger, iArgNeed, command.CommandName, sArgHelper);
                return ECommandAction.Stopped;
            }

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }

            if (!Int32.TryParse(command.GetArg(1), out int number)) number = 3;
            if (number >= 0 && number <= 3)
            {
                EW.g_EWPlayer[client].PFormatPlayer = number;

                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_PInfo_Format", number.ToString());
                }

                switch (number)
                {
                    case 1: UI.ReplyToCommand(client, "EntWatch.Reply.PlayerInfo.UserID", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : ""); break;
                    case 2: UI.ReplyToCommand(client, "EntWatch.Reply.PlayerInfo.SteamID", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : ""); break;
                    case 3: UI.ReplyToCommand(client, "EntWatch.Reply.PlayerInfo.Full", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : ""); break;
                    default: UI.ReplyToCommand(client, "EntWatch.Reply.PlayerInfo.NicknameOnly", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : ""); break;
                }
            }
            else UI.ReplyToCommand(client, "EntWatch.Reply.NotValid", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "");

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWChangeUsePriority(IGameClient client, StringCommand command)
        {
            if (!client.IsValid) return ECommandAction.Stopped;

            if (!EW.CheckDictionary(client))
            {
                UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                return ECommandAction.Stopped;
            }

            bool bNewValue = EW.g_EWPlayer[client].UsePriorityPlayer.Activate;

            string? sValue = command.ArgCount > 0 ? command.GetArg(1) : null;
            if (!string.IsNullOrEmpty(sValue))
            {
                bNewValue = sValue.Contains("true", StringComparison.OrdinalIgnoreCase) || string.Equals(sValue, "1");
            }
            else bNewValue = !bNewValue;

            EW.g_EWPlayer[client].UsePriorityPlayer.Activate = bNewValue;

            if (bNewValue)
            {
                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_Use_Priority", "1");
                }
                UI.ReplyToCommand(client, "EntWatch.Reply.Use_Priority.Enabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_enabled : "");
            }
            else
            {
                if (GetClientPrefs() is { } cp && cp.IsLoaded(client.SteamId))
                {
                    cp.SetCookie(client.SteamId, "EW_Use_Priority", "0");
                }
                UI.ReplyToCommand(client, "EntWatch.Reply.Use_Priority.Disabled", command.ChatTrigger, EW.g_Scheme != null ? EW.g_Scheme.Color_warning : "", EW.g_Scheme != null ? EW.g_Scheme.Color_disabled : "");
            }

            return ECommandAction.Stopped;
        }

        private ECommandAction OnEWStatus(IGameClient client, StringCommand command)
        {
            if (!client.IsValid || EW.g_Scheme == null) return ECommandAction.Stopped;

            var players = command.ArgCount >= 1 ? TargetManager.Find(client, command.GetArg(1)) : TargetManager.Find(client, "@me");

            if (players.Count > 0)
            {
                IGameClient target = players.Single();

                if (!EW.CheckDictionary(target))
                {
                    UI.ReplyToCommand(client, "EntWatch.Info.Error.NotFoundInDictionary", command.ChatTrigger);
                    return ECommandAction.Stopped;
                }

                if (EW.g_EWPlayer[target].BannedPlayer.bBanned)
                {
                    UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Has_a_Restrict", command.ChatTrigger, UI.PlayerInfo(client, UI.PlayerInfoFormat(target)), EW.g_Scheme.Color_disabled);
                    UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Admin", command.ChatTrigger, EW.g_Scheme.Color_warning, UI.PlayerInfo(client, UI.PlayerInfoFormat(EW.g_EWPlayer[target].BannedPlayer.sAdminName, EW.g_EWPlayer[target].BannedPlayer.sAdminSteamID)));
                    switch (EW.g_EWPlayer[target].BannedPlayer.iDuration)
                    {
                        case -1: UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Duration.Temporary", command.ChatTrigger, EW.g_Scheme.Color_warning, EW.g_Scheme.Color_enabled); break;
                        case 0: UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Duration.Permanently", command.ChatTrigger, EW.g_Scheme.Color_warning, EW.g_Scheme.Color_disabled); break;
                        default: UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Duration.Minutes", command.ChatTrigger, EW.g_Scheme.Color_warning, EW.g_Scheme.Color_disabled, EW.g_EWPlayer[target].BannedPlayer.iDuration); break;
                    }
                    UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Expires", command.ChatTrigger, EW.g_Scheme.Color_warning, EW.g_Scheme.Color_disabled, DateTimeOffset.FromUnixTimeSeconds(EW.g_EWPlayer[target].BannedPlayer.iTimeStamp_Issued));
                    UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Reason", command.ChatTrigger, EW.g_Scheme.Color_warning, EW.g_Scheme.Color_disabled, EW.g_EWPlayer[target].BannedPlayer.sReason);
                    UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Separator", command.ChatTrigger, EW.g_Scheme.Color_warning);
                }
                else UI.ReplyToCommand(client, "EntWatch.Reply.Eban.Can_pickup", command.ChatTrigger, UI.PlayerInfo(client, UI.PlayerInfoFormat(target)), EW.g_Scheme.Color_enabled);
            }
            else UI.ReplyToCommand(client, "EntWatch.Reply.No_matching_client", command.ChatTrigger, EW.g_Scheme.Color_warning);

            return ECommandAction.Stopped;
        }
    }
}
