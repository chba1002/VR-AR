using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using Assets.Scripts.Lobby.Mappers;

namespace Moth.Scripts.Lobby
{
    public class InsideRoomPanel : MonoBehaviour
    {
        public Button PlayerReadyButton;


        //public Button SelectMothBatButton;


        public GameObject PlayerSelectionPanelElements;
        private List<PlayerSelectionPanel> PlayerSelectionPanels;
        private bool isPlayerReady;
        //         public Image PlayerReadyImage;

        public GameObject MothPlayerListEntries;

        // Start is called before the first frame update
        void Start()
        {
            ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() {
                    {MothGame.PLAYER_READY, isPlayerReady},
                    {MothGame.PLAYER_LIVES, MothGame.PLAYER_MAX_LIVES},
                    {MothGame.PLAYER_MOTH_BAT_STATE, MothBatStateSerializer.Serialize(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE, false) }
                    };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);


            PlayerSelectionPanels = new List<PlayerSelectionPanel>();
            foreach (Transform child in PlayerSelectionPanelElements.transform)
            {
                var playerSelectionPanels = child.GetComponents<PlayerSelectionPanel>().ToList();
                PlayerSelectionPanels.AddRange(playerSelectionPanels);
            }
        }


        public void OnCLickPlayerReadyButton()
        {
            isPlayerReady = !isPlayerReady;
            //SetPlayerReady(isPlayerReady, PhotonNetwork.LocalPlayer.ActorNumber);

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            if (PhotonNetwork.IsMasterClient)
            {
                FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
            }
        }


        /// <summary>
        /// Try to set moth Id. After check if its settable. 
        /// </summary>
        /// <param name="mothBatId">Id of the selected moth or bat.</param>
        public void TrySetMothBat(int mothBatId)
        {
            if (mothBatId < 1 || (mothBatId > 4 && mothBatId != 100))
            {
                Debug.Log($"Unknown moth bat id {mothBatId}");
                return;
            }

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                if (!p.CustomProperties.TryGetValue(
                    MothGame.PLAYER_MOTH_BAT_STATE,
                    out object playerMothBatStateObject))
                {
                    continue;
                }

                // ToDo: Parse save

                var playerMothBatState = MothBatStateSerializer.Deserialize((string)playerMothBatStateObject);
      

                Debug.Log("parsedMothBatType: " + playerMothBatState.MothBatType + " => isSelected: " + playerMothBatState.IsSelected);

                bool elementIsUnselected = playerMothBatState.MothBatType == MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE;

                if (elementIsUnselected) continue;

                bool mothBatIdIsAlreadySelectedByLocalPlayer =
                    playerMothBatState.MothBatType == mothBatId &&
                    p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;

                if (mothBatIdIsAlreadySelectedByLocalPlayer)
                {
                    SetLocalPlayerMothBatIdInNetwork(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE, playerMothBatState.MothBatType, false);
                    UpdatePlayerSelectionPanelsSetMothBat(mothBatId, playerMothBatState.MothBatType, false);
                    Debug.Log("Set default moth bat type");
                    return;
                }

                bool alreadySelectedByOtherPlayer = playerMothBatState.MothBatType == mothBatId;

                if (alreadySelectedByOtherPlayer)
                {
                    Debug.Log($"The mothBat {mothBatId} is already selected.");
                    return;
                }
            }

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object localPlayerMothBatStateObject))
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

            SetLocalPlayerMothBatIdInNetwork(mothBatId, 0, true);
            UpdatePlayerSelectionPanelsSetMothBat(mothBatId, 0, true, PhotonNetwork.LocalPlayer.ActorNumber);

            //  if (PhotonNetwork.IsMasterClient)
            //      {
            //      FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
            //      }
        }

        private void SetLocalPlayerMothBatIdInNetwork(int mothBatId, int lastMothBatId, bool active)
        {
            Debug.Log($"Local: Spieler {PhotonNetwork.LocalPlayer.ActorNumber} wählt Motte {mothBatId} aus ({active})");
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { 
                { MothGame.PLAYER_MOTH_BAT_STATE,  MothBatStateSerializer.Serialize(mothBatId, active, lastMothBatId) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void UpdateMothPanelOfRemotePlayer(int mothBatId, int lastMothBatId, bool active, int? optionalPlayerId) => UpdatePlayerSelectionPanelsSetMothBat(mothBatId, lastMothBatId, active, optionalPlayerId);

        private void UpdatePlayerSelectionPanelsSetMothBat(int mothBatId, int lastMothBatId, bool active, int? optionalPlayerId = null)
        {
            //Debug.Log("UpdatePlayerSelectionPanelsSetMothBat mothBatId:" + mothBatId + " active:" + active + " [PlayerSelectionPanels.length: " + PlayerSelectionPanels.Count + "]");

            var allMothBatIdentifier = string.Join(" ", PlayerSelectionPanels);
            Debug.Log("allMothBatIdentifier: " + allMothBatIdentifier + " active: " + active +" ");

            PlayerSelectionPanels
                .Where(p => p.MothBatIdentifier == mothBatId || (lastMothBatId != 0 && mothBatId == 0 && optionalPlayerId.HasValue && p.OptionalPlayerId == optionalPlayerId.Value)) // Problem if mothBatId == 0
                .ToList()
                .ForEach(p => p.SetSelected(active, optionalPlayerId.HasValue ? optionalPlayerId.Value : null)
            );
        }

        internal void UpdateMothPanelOfRemotePlayerIsReady(int actorNumber, bool playerIsReady)
        {
            PlayerSelectionPanels
                .Where(p => p.OptionalPlayerId == actorNumber)
                .ToList()
                .ForEach(p => p.SetReady(playerIsReady)
            );
        }
    }
}