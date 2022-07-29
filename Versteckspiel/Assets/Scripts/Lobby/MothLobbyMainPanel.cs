using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moth.Scripts.Lobby.Managers;
using Assets.Scripts.Shared.Managers;
using Assets.Scripts.Text;
using Assets.Scripts.Shared;

namespace Moth.Scripts.Lobby // 350 rows
{
    public class MothLobbyMainPanel : MonoBehaviourPunCallbacks
    {
        private PlayerDataProvider playerDataProvider;

        [Header("Login Panel")]
        public GameObject LoginPanel;

        public TMP_Text PlayerName;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;
        public TMP_Text PlayerNameSelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;
        public GameObject MothPlayerListEntries;

       // public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        [Header("Hands")]
        public GameObject LeftHand;
        public GameObject RightHand;

        [Header("Testmode")]
        public TMP_Text TestOutput;
        public TMP_Text PingText;
        [SerializeField]
        private bool toStartGameMothAndBatMustBeSelected = true;
        #region UNITY

        private PlayerListManager playerListManager;
        private RoomListManager roomListManager;
        private NetworkManager networkManager;

        private PersonKey personKey;

        private float durationTillNextPingCheck = 1f;

        public void Awake()
        {
            playerDataProvider = new PlayerDataProvider();
        }

        public void Start()
        {
            networkManager = gameObject.GetComponent<NetworkManager>();
            networkManager.Initialize(TestOutput);
            TestOutput.gameObject.SetActive(false);

            playerListManager = new PlayerListManager(MothPlayerListEntries, PlayerListEntryPrefab, Instantiate, Destroy);
            roomListManager = new RoomListManager(
                Instantiate,
                Destroy,
                RoomListEntryPrefab,
                RoomListContent);


            SetActivePanel(LoginPanel.name);

            personKey = new PersonKey();
            PlayerName.text = personKey.Name;
            PlayerNameSelectionPanel.text = personKey.Name;
            ConnectToServer();
        }


        public void FixedUpdate()
        {
            durationTillNextPingCheck -= Time.deltaTime;

            if (durationTillNextPingCheck <= 0)
            {
                PingText.text = "Ping: "+PhotonNetwork.GetPing() + "ms";
                durationTillNextPingCheck = 1f;
            }
            
        }

        int GetPing() => PhotonNetwork.GetPing();


        private void ConnectToServer()
        {
            PhotonNetwork.LocalPlayer.NickName = personKey.Name;

            var props = new Hashtable
            {
                {MothGame.PLAYER_NAME, personKey.Name}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            //PhotonNetwork.ConnectUsingSettings();
            Debug.Log($"Try to connect '{personKey.Name}' to server...");

            bool success = PhotonNetwork.ConnectUsingSettings();


            var serverData = $"ServerAddress: (success: {success})" + PhotonNetwork.ServerAddress + " - Server: " + PhotonNetwork.Server + " UserId: " + PhotonNetwork.AuthValues?.UserId + " Token: " + PhotonNetwork.AuthValues?.Token;
            Debug.Log(serverData);

            TestOutput.text = serverData;

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
                //if (Input.GetKeyDown("s")) OnStartGameButtonClicked();
            }

            if (Input.GetKeyDown("h"))
            {
                LeftHand.SetActive(!LeftHand.activeSelf);
                RightHand.SetActive(!RightHand.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestOutput.gameObject.SetActive(!TestOutput.gameObject.activeSelf);
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

            //StartGameButton.gameObject.SetActive(CheckPlayersReady());

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
            //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        // 5. OnPlayerLeftRoom --> Remote Player left room
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log($"Remote: Spieler {otherPlayer.ActorNumber} verlässt Raum");
            playerListManager.Remove(otherPlayer.ActorNumber);

           // StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"OnRoomListUpdate: {roomList.Count} rooms");

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
                //StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            var playerTypeString = targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber
                ? $"LocalPlayer {targetPlayer.ActorNumber}:"
                : $"RemotePlayer {targetPlayer.ActorNumber}:";
            Debug.Log($"{playerTypeString} {targetPlayer.ActorNumber} Eigenschaften aktualisiert");


            // PLAYER_LIVES - Player is alive
            var changedPlayerData = playerDataProvider.Provide(targetPlayer, changedProps);

            // PLAYER_READY
            if (changedPlayerData.PlayerIsReady.HasValue)
            {
                var playerIsReady = changedPlayerData.PlayerIsReady.Value;
                Debug.Log($"{playerTypeString} ist bereit: '{playerIsReady}'.");

                playerListManager.SetPlayerReadyInUi(changedPlayerData);
                InsideRoomPanel
                    .GetComponent<InsideRoomPanel>()
                    .UpdateMothPanelOfRemotePlayerIsReady(changedPlayerData);


                var mothAndBatSelected = playerListManager
                    .CheckIfMinimumOneMothAndOneBatAreSelected(PhotonNetwork.PlayerList);

                if (!toStartGameMothAndBatMustBeSelected)
                {
                    mothAndBatSelected = true;
                }

                // Wenn nicht mindest 1 Fledermaus und 1 Motte ausgewählt,
                // Meldung anzeigen, dass diese mindestens ausgewählt sein müssen.
                if (!mothAndBatSelected)
                {
                    InsideRoomPanel.GetComponent<InsideRoomPanel>()
                        .SetInfoMessage(UiText.LOBBY_REQUIRES_BAT_AND_1_MOTH);
                }
                else if (!playerListManager.AllPlayersAreReady)
                {
                    // Meldung anzeigen. Sobald alle Spieler bereit sind startet spiel automatisch
                    InsideRoomPanel.GetComponent<InsideRoomPanel>()
                        .SetInfoMessage(UiText.LOBBY_WHEN_ALL_PLAYER_READY_GAME_STARTS);
                }
                else if (playerListManager.AllPlayersAreReady && mothAndBatSelected)
                {
                    OnStartGameButtonClicked();
                }
            }

            //  PLAYER_MOTH_BAT_TYPE
            if (changedPlayerData.PlayerMothBatState != null)
            {
                var playerMothBatState = changedPlayerData.PlayerMothBatState;
                Debug.Log($"('{changedPlayerData.PlayerName}'){playerTypeString} wählt Motte/Fledermaus " +
                    $"'{playerMothBatState.MothBatType}' ('{playerMothBatState.IsSelected}')  aus.");

                if (changedPlayerData.PlayerMothBatState.MothBatType == 0)
                {
                    playerListManager.Create(targetPlayer);
                }else if (changedPlayerData.PlayerMothBatState.MothBatType != 0 && changedPlayerData.PlayerMothBatState.LastMothBatType == 0)
                {
                    playerListManager.Remove(changedPlayerData.ActorNumber);
                }


                InsideRoomPanel
                    .GetComponent<InsideRoomPanel>()
                    .UpdateMothPanelOfRemotePlayer(changedPlayerData, targetPlayer);
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
            (var roomName, var roomOptions) = roomListManager.GetRoomProperties(RoomNameInputField.text, 5, Random.Range);

            var successfullyCreatedRoom = PhotonNetwork.CreateRoom(roomName, roomOptions);

            Debug.Log($"Successfully ({successfullyCreatedRoom}) created room {roomName}.");
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked() => PhotonNetwork.LeaveRoom();

        public void OnLoginButtonClicked()
        {
           // networkManager.Login(personKey.Name);
        }

        public void OnRoomListButtonClicked()
        {
            //if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(false ? "Versteckspiel" : "Test Scene");
        }

        #endregion

        //private bool CheckPlayersReady()
        //{
        //    return playerListManager.AllPlayersAreReady; //CheckAllPlayersAreReady(PhotonNetwork.PlayerList, PhotonNetwork.IsMasterClient);
        //}

        //public void LocalPlayerPropertiesUpdated() => StartGameButton.gameObject.SetActive(CheckPlayersReady());

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