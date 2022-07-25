using UnityEngine;
using Photon.Pun;
using Moth.Scripts;
using Assets.Scripts.Lobby.Mappers;

/// <summary>
/// Realize the initialization and orchestration of moths and bat 
/// </summary>
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    private MothGameManager mothGameManager;

    private void Awake()
    {
        // Debug.Log($"> PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}");
        // Debug.Log($"> PhotonNetwork.IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
        // Debug.Log($"> PhotonNetwork.IsMasterClient: {PhotonNetwork.IsMasterClient}");
        // Debug.Log($"> PhotonNetwork.LevelLoadingProgress: {PhotonNetwork.LevelLoadingProgress}");
    }

    private void Start()
    {
        mothGameManager = GameObject.FindGameObjectWithTag("MothGameManager")?.GetComponent<MothGameManager>();

        if(mothGameManager == null)
        {
            Debug.LogError("Coudn't find 'MothGameManager'.");
            return;
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(
            MothGame.PLAYER_MOTH_BAT_STATE,
            out object playerMothBatStateObject))
        {
            // ToDo: Parse save
            var playerMothBatState = MothBatStateSerializer.Deserialize((string)playerMothBatStateObject);

            Debug.Log("playerMothBatState.MothBatType: " + playerMothBatState.MothBatType);

            string playerPrefabName = "";

            switch (playerMothBatState.MothBatType)
            {
                case 1: playerPrefabName = "Moth_1_Network_Player"; break;
                case 2: playerPrefabName = "Moth_2_Network_Player"; break;
                case 3: playerPrefabName = "Moth_3_Network_Player"; break;
                case 4: playerPrefabName = "Moth_4_Network_Player"; break;
                case 5: playerPrefabName = "Bat_Network_Player"; break;
            }

            if(playerPrefabName == "")
            {
                Debug.LogError($"Unknown MothBatType {playerMothBatState.MothBatType}. Couldn't create player.");
                return;
            }

            var spawnPosition = mothGameManager.GetPositon(playerMothBatState.MothBatType);

            if (spawnPosition == Vector3.zero)
            {
                Debug.LogWarning($"Spawnpositon for {playerMothBatState.MothBatType} wasn't set.");
            }

            spawnedPlayerPrefab = PhotonNetwork.Instantiate(playerPrefabName, spawnPosition, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Couldn't find bat properties.");
        }
    }

    /*
     * ToDo: Evtl. wird hier eine Methode benötigt die remotePlayern das entsprechende Gameobject zuweist.
     * 
    public override void OnJoinedRoom()
    {
        Debug.Log($"Join {PhotonNetwork.LocalPlayer.ActorNumber}");
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Bat_Network_Player", transform.position, transform.rotation);
    }
    */


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
