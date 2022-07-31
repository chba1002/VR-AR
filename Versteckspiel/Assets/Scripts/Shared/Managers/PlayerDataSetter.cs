using ExitGames.Client.Photon;
using Moth.Scripts;
using Photon.Pun;

namespace Assets.Scripts.Shared.Managers
{
    public class PlayerDataSetter
    {
        public static void SetPlayerName(string playerName)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;

            var props = new Hashtable
            {
                {MothGame.PLAYER_NAME, playerName},
                {MothGame.PLAYER_MOTH_IS_INVULNERABLE, false }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public static void KillPlayerMoth(Photon.Realtime.Player player)
        {
            var props = new Hashtable
            {
                {MothGame.PLAYER_IS_ALIVE, false},
            };
            player.SetCustomProperties(props);
        }

        internal static void SetLevelLoaded(bool isLoaded)
        {
            var props = new Hashtable
            {
                {MothGame.PLAYER_LOADED_LEVEL, isLoaded}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}
