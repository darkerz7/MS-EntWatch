using Sharp.Shared.Definition;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace MS_EntWatch.Helpers
{
    public static class TargetManager
    {
        public static List<IGameClient> Find(IGameClient invoker, string? selector)
        {
            selector = (selector ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(selector)) return [];

            return selector.ToLowerInvariant() switch
            {
                "@me" => Me(invoker),
                "@all" or "@a" => All(),
                "@t" or "@te" => Team(CStrikeTeam.TE),
                "@ct" => Team(CStrikeTeam.CT),
                "@spec" => Team(CStrikeTeam.Spectator),
                "@alive" => LifeStateAlive(true),
                "@dead" => LifeStateAlive(false),
                "!@me" => NotMe(invoker),
                "@aim" => Aim(invoker),
                "@bots" => Bots(),
                _ => FindSmart(selector)
            };
        }

        private static List<IGameClient> Me(IGameClient controller)
        {
            return [controller];
        }

        private static List<IGameClient> All()
        {
            return [.. EntWatch._clients!.GetGameClients(true).ToList()];
        }

        private static List<IGameClient> Team(CStrikeTeam team)
        {
            List<IGameClient> matches = [];
            foreach (var player in EntWatch._entities!.GetPlayerControllers(true).Where(p => p.Team == team).ToList())
            {
                if (player.GetGameClient() is { } client)
                {
                    matches.Add(client);
                }
            }
            return matches;
        }
        private static List<IGameClient> LifeStateAlive(bool bAlive)
        {
            List<IGameClient> matches = [];
            foreach (var player in EntWatch._entities!.GetPlayerControllers(true).ToList())
            {
                if (player.GetPlayerPawn() is { } pawn && pawn.IsAlive == bAlive && player.GetGameClient() is { } client)
                {
                    matches.Add(client);
                }
            }
            return matches;
        }

        private static List<IGameClient> NotMe(IGameClient invoker)
        {
            return [.. All().Where(c => !ReferenceEquals(c, invoker))];
        }

        private static List<IGameClient> Aim(IGameClient invoker)
        {
            try
            {
                if (invoker.GetPlayerController() is { } player && player.GetPlayerPawn() is { IsAlive: true } pawn)
                {
                    var startPos = pawn.GetEyePosition();
                    var eyeAngles = pawn.GetEyeAngles();
                    var endPos = startPos + (eyeAngles.AnglesToVectorForward() * 2048);

                    var trace = EntWatch._physicsQuery!.TraceLine(startPos, endPos, UsefulInteractionLayers.FireBullets, CollisionGroupType.Default, TraceQueryFlag.All, InteractionLayers.None, pawn);

                    if (trace.HitEntity?.AsPlayerPawn() is { IsAlive: true } targetPawn)
                    {
                        if (targetPawn.GetController() is { ConnectedState: PlayerConnectedState.PlayerConnected } targetplayer && targetplayer.GetGameClient() is { } client)
                        {
                            return [client];
                        }
                    }
                }
            } catch{}

            return [];
        }

        private static List<IGameClient> Bots()
        {
            return [.. All().Where(c => c is { IsFakeClient: true})];
        }

        private static List<IGameClient> FindSmart(string selector)
        {
            if (selector[0] == '#')
            {
                var raw = selector[1..];
                if (string.IsNullOrEmpty(raw)) return [];

                //UserID
                if (int.TryParse(raw, out var UserID))
                {
                    foreach (var client in All())
                    {
                        if (client.UserId == UserID)
                        {
                            return [client];
                        }
                    }
                }

                //SteamID
                if (raw.StartsWith("steam", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var client in All())
                    {
                        if (raw.Equals(EW.ConvertSteamID64ToSteamID(client.SteamId.ToString()), StringComparison.OrdinalIgnoreCase))
                        {
                            return [client];
                        }
                    }
                }
            }

            // name contains
            return [.. All().Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(selector, StringComparison.OrdinalIgnoreCase))];
        }
    }
}
