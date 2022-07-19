using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

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
                    {MothGame.PLAYER_MOTH_BAT_TYPE, MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE}
                    };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                //SetPlayerReady(isPlayerReady, PhotonNetwork.LocalPlayer.ActorNumber);

                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
                }
            });

            PlayerSelectionPanels = new List<PlayerSelectionPanel>();
            foreach (Transform child in PlayerSelectionPanelElements.transform)
            {
                var playerSelectionPanels = child.GetComponents<PlayerSelectionPanel>().ToList();
                PlayerSelectionPanels.AddRange(playerSelectionPanels);
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
                    MothGame.PLAYER_MOTH_BAT_TYPE,
                    out object mothBatType))
                {
                    continue;
                }

                var parsedMothBatType = (int)mothBatType;

                //   Debug.Log("parsedMothBatType: " + parsedMothBatType);

                bool elementIsUnselected = parsedMothBatType == MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE;

                if (elementIsUnselected) continue;

                bool mothBatIdIsAlreadySelectedByLocalPlayer =
                    parsedMothBatType == mothBatId &&
                    p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;

                if (mothBatIdIsAlreadySelectedByLocalPlayer)
                {
                    SetLocalPlayerMothBatIdInNetwork(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE);
                    UpdatePlayerSelectionPanelsSetMothBat(mothBatId, false);
                    Debug.Log("Set default moth bat type");
                    return;
                }

                bool alreadySelectedByOtherPlayer = parsedMothBatType == mothBatId;

                if (alreadySelectedByOtherPlayer)
                {
                    Debug.Log($"The mothBat {mothBatId} is already selected.");
                    return;
                }
            }

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MothGame.PLAYER_MOTH_BAT_TYPE, out object mothBatTypeObject))
            {
                if (int.TryParse(mothBatTypeObject.ToString(), out int mothbatType))
                {
                    if (new List<int>() { 1, 2, 3, 4, 100 }.Contains(mothbatType))
                    {
                        Debug.Log("Couldnt select moth or bat. Already selected: "+ mothbatType);
                        return;
                    }
                }
            }

            SetLocalPlayerMothBatIdInNetwork(mothBatId);
            UpdatePlayerSelectionPanelsSetMothBat(mothBatId, true, PhotonNetwork.LocalPlayer.ActorNumber);

            //  if (PhotonNetwork.IsMasterClient)
            //      {
            //      FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
            //      }
        }

        private void SetLocalPlayerMothBatIdInNetwork(int mothBatId)
        {
            Debug.Log($"Local: Spieler {PhotonNetwork.LocalPlayer.ActorNumber} w�hlt Motte {mothBatId} aus");
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_MOTH_BAT_TYPE, mothBatId } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void UpdateMothPanelOfRemotePlayer(int mothBatId, bool active, int? optionalPlayerId) => UpdatePlayerSelectionPanelsSetMothBat(mothBatId, active, optionalPlayerId);


        private void UpdatePlayerSelectionPanelsSetMothBat(int mothBatId, bool active, int? optionalPlayerId = null)
        {
            //Debug.Log("UpdatePlayerSelectionPanelsSetMothBat mothBatId:" + mothBatId + " active:" + active + " [PlayerSelectionPanels.length: " + PlayerSelectionPanels.Count + "]");

            var allMothBatIdentifier = string.Join(" ", PlayerSelectionPanels);
            //  Debug.Log("allMothBatIdentifier: " + allMothBatIdentifier);

            PlayerSelectionPanels
                .Where(p => p.MothBatIdentifier == mothBatId)
                .ToList()
                .ForEach(p => p.SetSelected(active, optionalPlayerId)
            );
        }
    }
}