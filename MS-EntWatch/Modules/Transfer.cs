using MS_EntWatch.Helpers;
using MS_EntWatch.Items;
using Sharp.Shared.Objects;

namespace MS_EntWatch.Modules
{
    public static class Transfer
    {
        public static void Target(IGameClient admin, IGameClient? target, IGameClient receiver, bool bChat)
        {
            if (target == null) return;
            int iCount = 0;
            foreach (Item ItemTest in EW.g_ItemList)
            {
                if (ItemTest.Owner == target)
                {
                    ItemName(admin, ItemTest, receiver, bChat);
                    iCount++;
                }
            }
            if (iCount == 0 && EW.g_Scheme != null)
            {
                UI.ReplyToCommand(admin, "EntWatch.Reply.Transfer.NoItem", bChat, EW.g_Scheme.Color_warning);
            }
        }

        public static void ItemName(IGameClient admin, Item ItemTest, IGameClient receiver, bool bChat)
        {
            if (EW.g_Scheme == null) return;

            if (ItemTest.AllowTransfer != true || ItemTest.WeaponHandle is not { IsWeapon: true, IsValidEntity: true } weapon)
            {
                UI.ReplyToCommand(admin, "EntWatch.Reply.Transfer.NotAllow", bChat, EW.g_Scheme.Color_warning);
                return;
            }

            if (receiver.GetPlayerController() is { } receiverplayer && receiverplayer.GetPlayerPawn() is { } receiverpawn)
            {
                //DropWeapon from Receiver
                int iSlotIndex = GetSlotIndex(weapon.Classname);
                while (receiverpawn.GetWeaponBySlot(weapon.Slot, iSlotIndex) is { } wpn)
                {
                    foreach (Item ItemCheck in EW.g_ItemList.ToList())
                    {
                        if (wpn == ItemCheck.WeaponHandle)
                        {
                            UI.ReplyToCommand(admin, "EntWatch.Reply.Transfer.AlreadySlot", bChat, EW.g_Scheme.Color_warning);
                            return;
                        }
                    }
                    receiverpawn.DropWeapon(wpn);
                }
                //DropWeapon from target
                IGameClient? target = ItemTest.Owner;
                if (target is { IsValid:true } && target.GetPlayerController() is { } targetplayer && targetplayer.GetPlayerPawn() is { } targetpawn)
                {
                    ItemTest.Owner = null;
                    targetpawn.DropWeapon(weapon);
                }
                //Transfer
                weapon.SetAbsOrigin(receiverpawn.GetCenter());

                UI.EWChatAdminTransfer(UI.PlayerInfoFormat(admin), UI.PlayerInfoFormat(receiver), $"{ItemTest.Color}{ItemTest.Name}{EW.g_Scheme.Color_warning}", target != null ? UI.PlayerInfoFormat(target) : UI.PlayerInfoFormat("Console", "Server"));
                EW.g_cAPI.OnAdminTransferedItem(admin, ItemTest.Name, receiver);
            } else
            {
                 UI.ReplyToCommand(admin, "EntWatch.Reply.No_matching_client", bChat, EW.g_Scheme.Color_warning);
            }
        }

        private static int GetSlotIndex(string sEntityName)
        {
            int SlotIndex;
            switch (sEntityName)
            {
                case "weapon_hegrenade": { SlotIndex = 0; break; }
                case "weapon_flashbang": { SlotIndex = 1; break; }
                case "weapon_smokegrenade": { SlotIndex = 2; break; }
                case "weapon_decoy": { SlotIndex = 3; break; }
                case "weapon_incgrenade": { SlotIndex = 4; break; }
                case "weapon_molotov": { SlotIndex = 4; break; }
                case "weapon_knife": { SlotIndex = 0; break; }
                case "weapon_taser": { SlotIndex = 1; break; }
                default: { SlotIndex = -1; break; }
            }
            return SlotIndex;
        }
    }
}
