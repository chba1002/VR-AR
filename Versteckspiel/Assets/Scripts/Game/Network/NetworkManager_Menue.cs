using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager_Menue : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMPro.TMP_Text testOutput;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        Debug.Log("Connected To Server.");
        base.OnConnectedToMaster();

        var serverData = $"CONNECTED: " +
            $"ServerAdress {PhotonNetwork.ServerAddress} " +
            $"Region: {PhotonNetwork.CloudRegion} " +
            $"Server: {PhotonNetwork.Server} " +
            $"UserId: {PhotonNetwork.AuthValues?.UserId} ";

        Debug.Log(serverData);

        //testOutput.text = serverData;
     
        return;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate: " + roomList.Count);
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom!");
        base.OnCreatedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnCreateRoomFailed: {returnCode} :{message}");
        base.OnCreateRoomFailed(returnCode, message);
    }


    public override void OnJoinedRoom()
    {
        var serverData = $"OnJoinedRoom> ServerAddress: " + PhotonNetwork.ServerAddress + " - Server: " + PhotonNetwork.Server + " UserId: " + PhotonNetwork.AuthValues?.UserId + " Token: " + PhotonNetwork.AuthValues?.Token;
        Debug.Log(serverData);

        //testOutput.text = serverData;

        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("New Player Joined The Room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    internal void Initialize(TMPro.TMP_Text testOutput)
    {
        //this.testOutput = testOutput;
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
}
