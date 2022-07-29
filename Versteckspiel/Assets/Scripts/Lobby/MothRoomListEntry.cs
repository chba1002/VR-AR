using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Moth.Scripts.Lobby
{
    public class MothRoomListEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;
        public Button JoinRoomButton;

        private string roomName;
        private KeyCode number;

        public void Start()
        {
            JoinRoomButton.onClick.AddListener(() =>
            {
                OnJoinRoomButtonClicked();
            });
        }

        void Update()
        {
            if (Input.GetKeyDown(number)) OnJoinRoomButtonClicked();
        }

        public void Initialize(int number, string name, byte currentPlayers, byte maxPlayers)
        {
            switch (number)
            {
                case 1: this.number = KeyCode.Alpha1; break;
                case 2: this.number = KeyCode.Alpha2; break;
                case 3: this.number = KeyCode.Alpha3; break;
                case 4: this.number = KeyCode.Alpha4; break;
                default:
                    return;
            }

            roomName = name;
            RoomNameText.text = name;
            RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
        }

        private void OnJoinRoomButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
        }
    }
}