using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moth.Scripts.Lobby.Managers;
using System.Linq;

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

        #region UNITY

        private PlayerListManager playerListManager;
        private RoomListManager roomListManager;

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            playerListManager = new PlayerListManager(Instantiate, Destroy);
            roomListManager = new RoomListManager(
                Instantiate,
            Destroy,
            RoomListEntryPrefab,
            RoomListContent);

            PlayerName.text = "Spieler " + Random.Range(1000, 10000);
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster() => this.SetActivePanel(SelectionPanel.name);

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomListManager.ClearRoomListView();
            roomListManager.UpdateCachedRoomList(roomList);
            roomListManager.UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            roomListManager.ClearCachedRoomList();
            roomListManager.ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
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

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)

            roomListManager.ClearCachedRoomList();
            SetActivePanel(InsideRoomPanel.name);

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = playerListManager.InitiatePlayerListEntry(p, MothPlayerListEntries, PlayerListEntryPrefab);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out isPlayerReady))
                {
                    InsideRoomPanel.GetComponent<InsideRoomPanel>().SetPlayerReady((bool)isPlayerReady, p.ActorNumber);
                }

                playerListManager.PlayerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            var props = new Hashtable
            {
                {MothGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }


        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);
            playerListManager.PlayerListEntries.Values.ToList()
                .ForEach(entry => Destroy(entry.gameObject));
            playerListManager.ClearPlayerListEntries();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom newPlayer.ActorNumber: " + newPlayer.ActorNumber);
            GameObject entry = playerListManager.InitiatePlayerListEntry(newPlayer, MothPlayerListEntries, PlayerListEntryPrefab);
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            playerListManager.RemovePlayerListEntry(
                otherPlayer.ActorNumber,
                MothPlayerListEntries);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
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
            Debug.Log("OnPlayerPropertiesUpdate: ActorNumber: " + targetPlayer.ActorNumber);
            GameObject entry;
            Debug.Log(
                "playerListManager: "+playerListManager+
                ", playerListManager.PlayerListEntries: "+playerListManager.PlayerListEntries+
                ", targetPlayer: "+targetPlayer+
                ", targetPlayer.ActorNumber"+targetPlayer.ActorNumber);

            foreach (var item in playerListManager.PlayerListEntries)
            {
                Debug.Log("item "+item.Key +": "+item.Value);
            }    
            if (playerListManager.PlayerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                Debug.Log($"Player with actor number '{targetPlayer.ActorNumber}' was found.");

                object isPlayerReady;
                if (changedProps.TryGetValue(MothGame.PLAYER_READY, out isPlayerReady))
                {
                    Debug.Log($"Player isPlayerReady '{isPlayerReady}'.");

                    InsideRoomPanel.GetComponent<InsideRoomPanel>()
                        .SetPlayerReady((bool)isPlayerReady, targetPlayer.ActorNumber);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
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
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
            PhotonNetwork.CreateRoom(roomName, options, null);
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
            if (!PhotonNetwork.IsMasterClient) return false;
            return playerListManager.CheckPlayerIsReady(PhotonNetwork.PlayerList);
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