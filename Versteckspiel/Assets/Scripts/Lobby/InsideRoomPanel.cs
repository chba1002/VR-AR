using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Assets.Scripts.Lobby.Mappers;
using Assets.Scripts.Lobby.Managers;

namespace Moth.Scripts.Lobby
{
    public class InsideRoomPanel : MonoBehaviour
    {
        private bool isPlayerReady;

        private PlayerSelectionPanelListManager playerSelectionPanelManager;

        public Button PlayerReadyButton; 
        public GameObject PlayerSelectionPanelElements;
        public GameObject MothPlayerListEntries;

        void Start()
        {
            var initialProps = new ExitGames.Client.Photon.Hashtable() {
                    {MothGame.PLAYER_READY, isPlayerReady},
                    {MothGame.PLAYER_LIVES, MothGame.PLAYER_MAX_LIVES},
                    {MothGame.PLAYER_MOTH_BAT_STATE, MothBatStateSerializer.Serialize(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE, false) }
                    };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            playerSelectionPanelManager = new PlayerSelectionPanelListManager(PlayerSelectionPanelElements);
        }

        public void OnCLickPlayerReadyButton()
        {
            isPlayerReady = !isPlayerReady;

            var props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_READY, isPlayerReady } };
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
            var mothBatSetter = new MothBatSetter(
                PhotonNetwork.PlayerList,
                PhotonNetwork.LocalPlayer,
                SetLocalPlayerMothBatIdInNetwork,
                UpdatePlayerSelectionPanelsSetMothBat);

            mothBatSetter.Set(mothBatId);
        }

        private void SetLocalPlayerMothBatIdInNetwork(int mothBatId, int lastMothBatId, bool active)
        {
            Debug.Log($"Local: Spieler {PhotonNetwork.LocalPlayer.ActorNumber} wählt Motte {mothBatId} aus ({active})");
            var props = new ExitGames.Client.Photon.Hashtable() {
                { MothGame.PLAYER_MOTH_BAT_STATE,  MothBatStateSerializer.Serialize(mothBatId, active, lastMothBatId) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void UpdateMothPanelOfRemotePlayer(int mothBatId, int lastMothBatId, bool active, int? optionalPlayerId) 
            => UpdatePlayerSelectionPanelsSetMothBat(mothBatId, lastMothBatId, active, optionalPlayerId);

        private void UpdatePlayerSelectionPanelsSetMothBat(int mothBatId, int lastMothBatId, bool active, int? optionalPlayerId = null)
            => playerSelectionPanelManager.SetMothBat(mothBatId, lastMothBatId, active, optionalPlayerId);

        internal void UpdateMothPanelOfRemotePlayerIsReady(int actorNumber, bool playerIsReady)
            => playerSelectionPanelManager.UpdatePlayerIsReady(actorNumber, playerIsReady);
    }
}