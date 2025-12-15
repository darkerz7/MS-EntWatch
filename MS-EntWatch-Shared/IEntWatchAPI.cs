using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;

namespace MS_EntWatch_Shared
{
    public struct SEWAPI_Ban
    {
        public bool bBanned;                //True if user is banned, false otherwise

        public string sAdminName;           //Nickname admin who issued the ban
        public string sAdminSteamID;        //SteamID admin who issued the ban
        public int iDuration;               //Duration of the ban -1 - Temporary, 0 - Permamently, Positive value - time in minutes
        public int iTimeStamp_Issued;       //Pass an integer variable by reference and it will contain the UNIX timestamp when the player will be unbanned/ when a player was banned if ban = Permamently/Temporary
        public string sReason;              //The reason why the player was banned

        public string sClientName;          //Nickname of the player who got banned
        public string sClientSteamID;       //SteamID of the player who got banned

        public SEWAPI_Ban()
        {
            bBanned = false;
            sAdminName = "Console";
            sAdminSteamID = "SERVER";
            iDuration = 0;
            iTimeStamp_Issued = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            sReason = "No Reason";
            sClientName = "";
            sClientSteamID = "";
        }
    }
    public interface IEntWatchAPI
    {
        const string Identity = nameof(IEntWatchAPI);

        /**
		 * Checks if a player is currently banned, if an integer variable is referenced the time of unban will be assigned to it.
		 *
		 * @param sSteamID		SteamID of the player to check for ban
		 * @return				Event SEWAPI_Ban struct
		 *
		 */
        void Native_EntWatch_IsClientBanned(string sSteamID);

        /**
		 * Bans a player from using special items.
		 *
		 * @param sewPlayer		SEWAPI_Ban struct to ban
		 *
		 * On error/errors:		Invalid player
		 */
        void Native_EntWatch_BanClient(SEWAPI_Ban sewPlayer);

        /**
		 * Unbans a previously ebanned player.
		 *
		 * @param sewPlayer		SEWAPI_Ban struct to unban
		 *
		 * On error/errors:		Invalid player
		 */
        void Native_EntWatch_UnbanClient(SEWAPI_Ban sewPlayer);

        /**
		 * Forces a ban status update.
		 *
		 * @param Player		IGameClient for forced update
		 *
		 * On error/errors:		Invalid player
		 */
        void Native_EntWatch_UpdateStatusBanClient(IGameClient Player);

        /**
		 * Checks if an entity is a special item.
		 *
		 * @param cEntity		IBaseWeapon to check
		 * @return				True if entity is a special item, false otherwsie
		 */
        bool Native_EntWatch_IsSpecialItem(IBaseWeapon cEntity);

        /**
		 * Checks if an entity is a special item button.
		 *
		 * @param cEntity		IBaseEntity to check
		 * @return				True if entity is a special item, false otherwsie
		 */
        bool Native_EntWatch_IsButtonSpecialItem(IBaseEntity cEntity);

        /**
		 * Checks if a player has a special item.
		 *
		 * @param Player		Player to check
		 * @return				True if player has a special item, false otherwsie
		 */
        bool Native_EntWatch_HasSpecialItem(IGameClient Player);

        /**
		 * Allows special items to glow for the player.
		 *
		 * @param Player		IGameClient for forced update
		 *
		 * On error/errors:		Invalid player
		 */
        void Native_EntWatch_EnableWeaponGlow(IGameClient Player);

        /**
		 * Prevents special items to glow for the player.
		 *
		 * @param Player		IGameClient for forced update
		 *
		 * On error/errors:		Invalid player
		 */
        void Native_EntWatch_DisableWeaponGlow(IGameClient Player);

        /**
		 * Called when a player is e-banned by any means
		 *
		 * @param sewPlayer		Full information about ban in SEWAPI_Ban struct
		 *
		 * @return				None
		 */
        public delegate void Forward_OnClientBanned(SEWAPI_Ban sewPlayer);
        public event Forward_OnClientBanned Forward_EntWatch_OnClientBanned;

        /**
		 * Called when a player is e-unbanned by any means
		 *
		 * @param sewPlayer		Full information about unban in SEWAPI_Ban struct
		 * @return				None
		 */
        public delegate void Forward_OnClientUnbanned(SEWAPI_Ban sewPlayer);
        public event Forward_OnClientUnbanned Forward_EntWatch_OnClientUnbanned;

        /**
		 * Сalled when a player is use item
		 *
		 * @param sItemName		The name of the item that was used
		 * @param Player		IGameClient that was used item
		 * @param sAbility		The ability name of the item that was used
		 * @return				None
		 */
        public delegate void Forward_OnUseItem(string sItemName, IGameClient Player, string sAbility);
        public event Forward_OnUseItem Forward_EntWatch_OnUseItem;

        /**
		 * Сalled when a player is pickup item
		 *
		 * @param sItemName		The name of the item that was picked
		 * @param Player		IGameClient that was picked item
		 * @return				None
		 */
        public delegate void Forward_OnPickUpItem(string sItemName, IGameClient Player);
        public event Forward_OnPickUpItem Forward_EntWatch_OnPickUpItem;

        /**
		 * Сalled when a player is drop item
		 *
		 * @param sItemName		The name of the item that was dropped
		 * @param Player		IGameClient that was dropped item
		 * @return				None
		 */
        public delegate void Forward_OnDropItem(string sItemName, IGameClient Player);
        public event Forward_OnDropItem Forward_EntWatch_OnDropItem;

        /**
		 * Сalled when a player is disconnect with item
		 *
		 * @param sItemName		The name of the item that the disconnected player had
		 * @param Player		IGameClient that was disconnected with item
		 * @return				None
		 */
        public delegate void Forward_OnPlayerDisconnectWithItem(string sItemName, IGameClient Player);
        public event Forward_OnPlayerDisconnectWithItem Forward_EntWatch_OnPlayerDisconnectWithItem;

        /**
		 * Сalled when a player is death with item
		 *
		 * @param sItemName		The name of the item that player had when death
		 * @param Player		IGameClient that was death with item
		 * @return				None
		 */
        public delegate void Forward_OnPlayerDeathWithItem(string sItemName, IGameClient Player);
        public event Forward_OnPlayerDeathWithItem Forward_EntWatch_OnPlayerDeathWithItem;

        /**
		 * Called when a admin was spawned item
		 *
		 * @param Admin			Admin IGameClient that was spawned item
		 * @param sItemName		The name of the item that was spawned
		 * @param Target		Target IGameClient that received the item
		 *
		 * @return				None
		 */
        public delegate void Forward_OnAdminSpawnItem(IGameClient Admin, string sItemName, IGameClient Target);
        public event Forward_OnAdminSpawnItem Forward_EntWatch_OnAdminSpawnItem;

        /**
		 * Called when a admin was transfered item to receiver
		 *
		 * @param Admin		Admin IGameClient that was transfered item
		 * @param sItemName		The name of the item transfered to the receiver
		 * @param Receiver		Receiver IGameClient that received the item
		 *
		 * @return				None
		 */
        public delegate void Forward_OnAdminTransferedItem(IGameClient Admin, string sItemName, IGameClient Target);
        public event Forward_OnAdminTransferedItem Forward_EntWatch_OnAdminTransferedItem;

        /**
		 * Called when response received from database
		 *
		 * @param sewPlayer		Full information about client in SEWAPI_Ban struct
		 *
		 * @return				None
		 */
        public delegate void Forward_IsClientBannedResult(SEWAPI_Ban sewPlayer);
		public event Forward_IsClientBannedResult Forward_EntWatch_IsClientBannedResult;
    }
}
