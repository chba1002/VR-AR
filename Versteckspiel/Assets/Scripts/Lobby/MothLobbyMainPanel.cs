using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moth.Scripts.Lobby.Managers;

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

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;

        #region UNITY

        private PlayerListManager playerListManager;

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            playerListManager = new PlayerListManager(Instantiate, Destroy);

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();

            PlayerName.text = "Spieler " + Random.Range(1000, 10000);
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }



        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions { MaxPlayers = 8 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();


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

            Hashtable props = new Hashtable
            {
                {MothGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }


        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListManager.PlayerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

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

        // ToDo: OnPlayerJoined - Refresh User List

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log("OnPlayerPropertiesUpdate: ActorNumber: " + targetPlayer.ActorNumber);


            GameObject entry;
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
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

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

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

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
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

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
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<MothRoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }
    }
}