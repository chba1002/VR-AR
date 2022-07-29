using Assets.Scripts.Lobby.Mappers;
using Assets.Scripts.Shared.Types;
using ExitGames.Client.Photon;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;
using Photon.Pun;
using System;
using UnityEngine;

namespace Assets.Scripts.Shared.Managers
{
    public class PlayerDataSetter
    {
        public static void SetPlayerName(string playerName)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;

            var props = new Hashtable
            {
                {MothGame.PLAYER_NAME, playerName}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
