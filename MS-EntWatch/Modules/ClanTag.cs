using MS_EntWatch.Items;
using Sharp.Shared.GameEntities;

namespace MS_EntWatch.Modules
{
    static class ClanTag
    {
        public static void UpdateClanTag()
        {
            if (Cvar.ClanTag && Cvar.ClanTagInfo)
            {
                foreach (Item ItemTest in EW.g_ItemList.ToList())
                {
                    if (ItemTest.Owner != null) ConstructClanTag(ItemTest);
                }
            }
        }

        public static void UpdatePickUp(Item ItemTest)
        {
            if (Cvar.ClanTag)
            {
                ConstructClanTag(ItemTest);
            }
        }

        public static void RemoveClanTag(IPlayerController player)
        {
            if (Cvar.ClanTag)
            {
                SetClan(player, "");
            }
        }

        private static void ConstructClanTag(Item ItemTest)
        {
            if (ItemTest.Owner?.GetPlayerController() is { } player)
            {
                string sClanTag = $"{ItemTest.ShortName}";
                if (Cvar.ClanTagInfo)
                {
                    sClanTag += " ";
                    if (ItemTest.CheckDelay())
                    {
                        int iAbilityCount = 0;
                        foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                        {
                            if (++iAbilityCount > Cvar.DisplayAbility) break;
                            if (!AbilityTest.Ignore) sClanTag += $"[{AbilityTest.GetMessage()}]";
                        }

                    }
                    else sClanTag += $"[-{Math.Round(ItemTest.fDelay - EW.fGameTime, 1)}]";
                }
                SetClan(player, sClanTag);
            }
        }

        private static void SetClan(IPlayerController player, string sClanTag)
        {
            if (sClanTag.Length > 24) player.SetClanTag(sClanTag[..23]);
            else player.SetClanTag(sClanTag);

            if (EntWatch._events!.CreateEvent("nextlevel_changed", false) is { } fakeEvent) fakeEvent.FireToClient(player.PlayerSlot);
        }
    }
}
