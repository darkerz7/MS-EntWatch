using MS_EntWatch.Items;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace MS_EntWatch.Modules
{
    internal class UsePriority
    {
        public bool Activate;

        bool LockSpam;
        bool OneButton;
        Item? OneItem;
        public UsePriority()
        {
            Activate = true;
            LockSpam = false;
            OneButton = false;
            OneItem = null;
        }

        public void DetectUse(IGameClient client)
        {
            if (!EW.g_EWPlayer[client].UsePriorityPlayer.Activate || LockSpam || !OneButton) return;
            LockSpam = true;
            EntWatch._modSharp!.PushTimer(() => LockSpam = false, 0.5f, GameTimerFlags.Repeatable);

            int iNum = 0;

            if (OneItem is { } && client.GetPlayerController() is { } player && player.GetPawn() is { } pawn)
            {
                foreach (Ability AbilityTest in OneItem.AbilityList.ToList())
                {
                    if (AbilityTest.Ignore)
                    {
                        iNum++;
                        continue;
                    }
                    break;
                }
                if (iNum + 1 > OneItem.AbilityList.Count) return; //All Ignore

                if (OneItem.CheckDelay() && OneItem.AbilityList[iNum] is { } ability && ability.Mode != 1 && ability.Mode < 6 && ability.fLastUse < EW.fGameTime && ability.Entity != null && ability.Entity.IsValid() && !ability.LockItem)
                {
                    ability.Entity.AcceptInput("Use", pawn, pawn);
                }
            }
        }
        public void UpdateCountButton(IGameClient UPlayer)
        {
            OneButton = false;
            int iCount = 0;
            foreach (Item ItemTest in EW.g_ItemList.ToList())
            {
                if (ItemTest.Owner == UPlayer)
                {
                    int iCountWithoutIgnore = 0;
                    foreach (Ability AbilityTest in ItemTest.AbilityList.ToList())
                        if (!AbilityTest.Ignore && AbilityTest.Mode != 8) iCountWithoutIgnore++;
                    iCount += iCountWithoutIgnore;
                    if (!ItemTest.UsePriority || iCount > 1)
                    {
                        OneButton = false;
                        OneItem = null;
                        break;
                    }
                    if (iCount == 1)
                    {
                        OneButton = true;
                        OneItem = ItemTest;
                    }
                }
            }
        }
    }
}
