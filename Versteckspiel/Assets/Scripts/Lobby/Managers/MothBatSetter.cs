using Assets.Scripts.Lobby.Mappers;
using Assets.Scripts.Shared.Managers;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Lobby.Managers
{
    class MothBatSetter
    {
        private Photon.Realtime.Player[] playerList;
        private Photon.Realtime.Player localPlayer;
        private System.Action<int, int, bool> setLocalPlayerMothBatIdInNetwork;
        private System.Action<PlayerData, Photon.Realtime.Player> updatePlayerSelectionPanelsSetMothBat;
        private GameObject playerReadyButton_go;
        private PlayerDataProvider playerDataProvider;

        public MothBatSetter(
            Photon.Realtime.Player[] playerList, 
            Photon.Realtime.Player localPlayer, 
            System.Action<int, int, bool> setLocalPlayerMothBatIdInNetwork, 
            System.Action<PlayerData, Photon.Realtime.Player> updatePlayerSelectionPanelsSetMothBat,
            GameObject playerReadyButton_go)
        {
            this.playerDataProvider = new PlayerDataProvider();
            this.playerList = playerList;
            this.localPlayer = localPlayer;
            this.setLocalPlayerMothBatIdInNetwork = setLocalPlayerMothBatIdInNetwork;
            this.updatePlayerSelectionPanelsSetMothBat = updatePlayerSelectionPanelsSetMothBat;
            this.playerReadyButton_go = playerReadyButton_go;
        }

        public void Set(int mothBatId)
        {
            if (mothBatId < 1 || (mothBatId > 4 && mothBatId != 100))
            {
                Debug.Log($"Unknown moth bat id {mothBatId}");
                return;
            }

            foreach (Photon.Realtime.Player p in playerList)
            {
                var playerData = playerDataProvider.Provide(p);

                if (playerData.PlayerMothBatState == null)
                {
                    continue;
                }

                var playerMothBatState = playerData.PlayerMothBatState;


                Debug.Log("parsedMothBatType: " + playerMothBatState.MothBatType + " => isSelected: " + playerMothBatState.IsSelected);

                bool elementIsUnselected = playerMothBatState.MothBatType == MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE;

                if (elementIsUnselected) continue;

                bool mothBatIdIsAlreadySelectedByLocalPlayer =
                    playerMothBatState.MothBatType == mothBatId &&
                    p.ActorNumber == localPlayer.ActorNumber;

                if (mothBatIdIsAlreadySelectedByLocalPlayer)
                {
                    // ToDo: ?? Does this line makes sense?
                    setLocalPlayerMothBatIdInNetwork(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE, playerMothBatState.MothBatType, false);

                    var newPlayerMothBatState = new PlayerMothBatState()
                    {
                        MothBatType = mothBatId,
                        LastMothBatType = playerMothBatState.MothBatType,
                        IsSelected = false
                    };

                    playerData.SetPlayerMothBatState(newPlayerMothBatState);

                    updatePlayerSelectionPanelsSetMothBat(playerData, null);
                    Debug.Log("Set default moth bat type");
                    playerReadyButton_go.SetActive(false);
                    return;
                }

                bool alreadySelectedByOtherPlayer = playerMothBatState.MothBatType == mothBatId;

                if (alreadySelectedByOtherPlayer)
                {
                    Debug.Log($"The mothBat {mothBatId} is already selected.");
                    return;
                }
            }

            if (localPlayer.CustomProperties.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object localPlayerMothBatStateObject))
            {
                // ToDo: Parse save
                var playerMothBatState = MothBatStateSerializer.Deserialize(((string)localPlayerMothBatStateObject));
                var parsedMothBatType = playerMothBatState.MothBatType;

                if (new List<int>() { 1, 2, 3, 4, 100 }.Contains(parsedMothBatType))
                {
                    Debug.Log("Couldn't select moth or bat. Already selected: " + parsedMothBatType);
                    return;
                }
            }

            setLocalPlayerMothBatIdInNetwork(mothBatId, 0, true);

            var localPlayerData = playerDataProvider.Provide(localPlayer);

            var localPlayerMothBatState = new PlayerMothBatState()
            {
                MothBatType = mothBatId,
                LastMothBatType = 0,
                IsSelected = true
            };

            localPlayerData.SetPlayerMothBatState(localPlayerMothBatState);

            updatePlayerSelectionPanelsSetMothBat(localPlayerData, localPlayer);


            playerReadyButton_go.SetActive(true);

            //  if (localPlayer)
            //      {
            //      FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
            //      }
        }
    }
}
