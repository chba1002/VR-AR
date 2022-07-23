using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moth.Scripts.Lobby.Managers;
using System.Linq;
using Assets.Scripts.Lobby.Mappers;

namespace Moth.Scripts.Lobby
{
    public class MothLobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public TMP_Text PlayerName;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;
        public GameObject MothPlayerListEntries;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        [Header("Handsl")]
        public GameObject LeftHand;
        public GameObject RightHand;

        #region UNITY

        private PlayerListManager playerListManager;
        private RoomListManager roomListManager;

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            playerListManager = new PlayerListManager(MothPlayerListEntries, PlayerListEntryPrefab, Instantiate, Destroy);
            roomListManager = new RoomListManager(
                Instantiate,
            Destroy,
            RoomListEntryPrefab,
            RoomListContent);

            PlayerName.text = "Spieler " + Random.Range(1000, 10000);
            SetActivePanel(LoginPanel.name);
        }

        void Update()
        {
            if (LoginPanel.activeSelf)
            {
                if (Input.GetKeyDown("l")) OnLoginButtonClicked();
            }
            else if (SelectionPanel.activeSelf)
            {
                if (Input.GetKeyDown("s")) OnCreateRoomButtonClicked();
                if (Input.GetKeyDown("v")) OnRoomListButtonClicked();
            }
            else if (InsideRoomPanel.activeSelf)
            {
                if (Input.GetKeyDown("1")) InsideRoomPanel.GetComponent<InsideRoomPanel>().TrySetMothBat(1);
                if (Input.GetKeyDown("2")) InsideRoomPanel.GetComponent<InsideRoomPanel>().TrySetMothBat(2);
                if (Input.GetKeyDown("3")) InsideRoomPanel.GetComponent<InsideRoomPanel>().TrySetMothBat(3);
                if (Input.GetKeyDown("4")) InsideRoomPanel.GetComponent<InsideRoomPanel>().TrySetMothBat(4);
                if (Input.GetKeyDown("5")) InsideRoomPanel.GetComponent<InsideRoomPanel>().TrySetMothBat(100);
                if (Input.GetKeyDown("b")) InsideRoomPanel.GetComponent<InsideRoomPanel>().OnCLickPlayerReadyButton();
                if (Input.GetKeyDown("v")) OnLeaveGameButtonClicked();
                if (Input.GetKeyDown("s")) OnStartGameButtonClicked();
            }

            if (Input.GetKeyDown("h"))
            {
                LeftHand.SetActive(!LeftHand.activeSelf);
                RightHand.SetActive(!RightHand.activeSelf);
            }
        }

        #endregion

        #region PUN CALLBACKS

        // 2. On ConnectedToMaster --> Connected
        public override void OnConnectedToMaster() => this.SetActivePanel(SelectionPanel.name);

        // 3. On OnJoinedRoom --> Player joined
        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)

            roomListManager.ClearCachedRoomList();
            SetActivePanel(InsideRoomPanel.name);

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                playerListManager.Create(p);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            var props = new Hashtable
            {
                {MothGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // 4. OnPlayerEnteredRoom --> Remote Player joined
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newRemotePlayer)
        {
            Debug.Log($"Remote: Spieler {newRemotePlayer.ActorNumber} betritt Raum");
            playerListManager.Create(newRemotePlayer);
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        // 5. OnPlayerLeftRoom --> Remote Player left room
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log($"Remote: Spieler {otherPlayer.ActorNumber} verlässt Raum");
            playerListManager.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomListManager.ClearRoomListView();
            roomListManager.UpdateCachedRoomList(roomList);
            roomListManager.UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby: Player enters Lobby");
            // whenever this joins a new lobby, clear any previous room lists
            roomListManager.ClearCachedRoomList();
            roomListManager.ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby: Player left Lobby");
            roomListManager.ClearCachedRoomList();
            roomListManager.ClearRoomListView();
        }
        public override void OnCreateRoomFailed(short returnCode, string message) =>
            SetActivePanel(SelectionPanel.name);

        public override void OnJoinRoomFailed(short returnCode, string message) =>
            SetActivePanel(SelectionPanel.name);

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(
                roomListManager.GenerateRoomName(),
                new RoomOptions { MaxPlayers = 8 },
                null);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom");
            SetActivePanel(SelectionPanel.name);
            playerListManager.ClearPlayerListEntries();
        }


        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            var playerTypeString = targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber
                ? $"LocalPlayer {targetPlayer.ActorNumber}:"
                : $"RemotePlayer {targetPlayer.ActorNumber}:";
            Debug.Log($"{playerTypeString} {targetPlayer.ActorNumber} Eigenschaften aktualisiert");


            // PLAYER_LIVES - Player is alive

            // PLAYER_READY
            if (changedProps.TryGetValue(MothGame.PLAYER_READY, out object isPlayerReady))
            {
                Debug.Log($"{playerTypeString}  ist bereit: '{isPlayerReady}'.");

                playerListManager.SetPlayerReadyInUi((bool)isPlayerReady, targetPlayer.ActorNumber);

                InsideRoomPanel
                    .GetComponent<InsideRoomPanel>()
                    .UpdateMothPanelOfRemotePlayerIsReady(targetPlayer.ActorNumber, (bool)isPlayerReady);

                if (playerListManager.AllPlayersAreReady)
                {
                    OnStartGameButtonClicked();
                }
            }

            //  PLAYER_MOTH_BAT_TYPE
            if (changedProps.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object playerMothBatStateObject))
            {
                // ToDo: Parse save
                var playerMothBatState = MothBatStateSerializer.Deserialize(((string)playerMothBatStateObject));

                if (playerMothBatState == null)
                {
                    Debug.LogError($"{playerMothBatState.MothBatType} konnte nicht geparsed werden.");
                    return;
                }

                Debug.Log($"{playerTypeString} wählt Motte/Fledermaus '{playerMothBatState.MothBatType}' ('{playerMothBatState.IsSelected}') aus.");

                InsideRoomPanel
                    .GetComponent<InsideRoomPanel>()
                    .UpdateMothPanelOfRemotePlayer(playerMothBatState.MothBatType, playerMothBatState.LastMothBatType, playerMothBatState.IsSelected, targetPlayer.ActorNumber);
            }

            //  PLAYER_LOADED_LEVEL




            // ToDo: Check  if all player are ready and if moths AND bats are selected
            // ToDo: Should be done via 'PlayerListManager'
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
            SetActivePanel(SelectionPanel.name);
        }

        public void OnCreateRoomButtonClicked()
        {
            (var roomName, var roomOptions) = roomListManager.CreateRoom(RoomNameInputField.text, MaxPlayersInputField.text, Random.Range);
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked() => PhotonNetwork.LeaveRoom();

        public void OnLoginButtonClicked()
        {
            string playerName = PlayerName.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Versteckspiel");
        }

        #endregion

        private bool CheckPlayersReady()
        {
            return playerListManager.AllPlayersAreReady; //CheckAllPlayersAreReady(PhotonNetwork.PlayerList, PhotonNetwork.IsMasterClient);
        }

        public void LocalPlayerPropertiesUpdated() =>
            StartGameButton.gameObject.SetActive(CheckPlayersReady());

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }
    }
}