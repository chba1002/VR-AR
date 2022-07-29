using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_Test : MonoBehaviourPunCallbacks
{
    private TMPro.TMP_Text testOutput;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try connect to Server");
    }

    public override void OnJoinedRoom()
    {
        var serverData = $"OnJoinedRoom> ServerAddress: " + PhotonNetwork.ServerAddress + " - Server: " + PhotonNetwork.Server + " UserId: " + PhotonNetwork.AuthValues?.UserId + " Token: " + PhotonNetwork.AuthValues?.Token;
        Debug.Log(serverData);

       // testOutput.text = serverData;

        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("OnPlayerConnected: " + player.name);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("A new Player joined the Room");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
