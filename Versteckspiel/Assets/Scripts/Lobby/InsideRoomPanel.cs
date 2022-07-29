using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Assets.Scripts.Lobby.Mappers;
using Assets.Scripts.Lobby.Managers;
using Assets.Scripts.Shared.Managers;
using TMPro;
using Assets.Scripts.Shared.Types;

namespace Moth.Scripts.Lobby
{
    public class InsideRoomPanel : MonoBehaviour
    {
        private bool isPlayerReady;

        private PlayerDataProvider playerDataProvider;
        private PlayerSelectionPanelListManager _playerSelectionPanelManager;

        private PlayerSelectionPanelListManager playerSelectionPanelManager
        {
            get
            {
                if(_playerSelectionPanelManager == null) _playerSelectionPanelManager = new PlayerSelectionPanelListManager(PlayerSelectionPanelElements);
                return _playerSelectionPanelManager;
            }
        }

        [SerializeField]
        private TMP_Text InfoMessage;


        public Button PlayerReadyButton; 
        public GameObject PlayerSelectionPanelElements;
        public GameObject TopMothPlayerListEntries;

        void Start()
        {
            playerDataProvider = new PlayerDataProvider();
            PlayerReadyButton.gameObject.SetActive(false);
            SetInfoMessage("-");
            var initialProps = new ExitGames.Client.Photon.Hashtable() {
                    {MothGame.PLAYER_READY, isPlayerReady},
                    {MothGame.PLAYER_LIVES, MothGame.PLAYER_MAX_LIVES},
                    {MothGame.PLAYER_MOTH_BAT_STATE, MothBatStateSerializer.Serialize(MothGame.PLAYER_DEFAULT_MOTH_BAT_TYPE, false) }
                    };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            _playerSelectionPanelManager = new PlayerSelectionPanelListManager(PlayerSelectionPanelElements);
        }

        public void OnCLickPlayerReadyButton()
        {
            var playerData = playerDataProvider.Provide(PhotonNetwork.LocalPlayer);

            if(playerData?.PlayerMothBatState?.MothBatType == null 
                || playerData.PlayerMothBatState.MothBatType == 0) {
                return;
            }

            isPlayerReady = !isPlayerReady;

            var props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            if (PhotonNetwork.IsMasterClient)
            {
                //FindObjectOfType<MothLobbyMainPanel>().LocalPlayerPropertiesUpdated();
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
                UpdatePlayerSelectionPanelsSetMothBat,
                PlayerReadyButton.gameObject);

            
            mothBatSetter.Set(mothBatId);
        }

        public void SetInfoMessage(string infoMessage)
        {
            InfoMessage.text = infoMessage;
        }

        private void SetLocalPlayerMothBatIdInNetwork(int mothBatId, int lastMothBatId, bool active)
        {
            Debug.Log($"Local: Spieler {PhotonNetwork.LocalPlayer.ActorNumber} wählt Motte {mothBatId} aus ({active})");
            var props = new ExitGames.Client.Photon.Hashtable() {
                { MothGame.PLAYER_MOTH_BAT_STATE,  MothBatStateSerializer.Serialize(mothBatId, active, lastMothBatId) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void UpdateMothPanelOfRemotePlayer(PlayerData playerData, Photon.Realtime.Player optionalPlayer)
        {
            UpdatePlayerSelectionPanelsSetMothBat(playerData, optionalPlayer);
        }
         
        private void UpdatePlayerSelectionPanelsSetMothBat(PlayerData playerData, Photon.Realtime.Player optionalPlayer = null)
            => playerSelectionPanelManager.SetMothBat(playerData, optionalPlayer);

        internal void UpdateMothPanelOfRemotePlayerIsReady(PlayerData playerData)
        {
            if (!playerData.PlayerIsReady.HasValue) return;
            playerSelectionPanelManager.UpdatePlayerIsReady(playerData.ActorNumber, playerData.PlayerIsReady.Value);
        }
    }
}