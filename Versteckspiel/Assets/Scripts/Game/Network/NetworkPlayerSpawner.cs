using UnityEngine;
using Photon.Pun;
using Moth.Scripts;
using Assets.Scripts.Lobby.Mappers;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    private void Awake()
    {
        Debug.Log($"> PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}");
        Debug.Log($"> PhotonNetwork.IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
        Debug.Log($"> PhotonNetwork.IsMasterClient: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"> PhotonNetwork.LevelLoadingProgress: {PhotonNetwork.LevelLoadingProgress}");

    }


    private void Start()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(
            MothGame.PLAYER_MOTH_BAT_STATE,
            out object playerMothBatStateObject))
        {
            // ToDo: Parse save
            var playerMothBatState = MothBatStateSerializer.Deserialize((string)playerMothBatStateObject);

            Debug.Log("playerMothBatState.MothBatType: " + playerMothBatState.MothBatType);
        }



        Debug.Log($"1 Join {PhotonNetwork.LocalPlayer.ActorNumber}");

        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Join {PhotonNetwork.LocalPlayer.ActorNumber}");
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
