using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Assets.Scripts.Shared.Managers;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;

public class MothBatNetworkSynchronizer : MonoBehaviourPunCallbacks
{
    private PlayerDataProvider playerDataProvider;

    public GameObject TestPostprocessing;

    void Start()
    {
        playerDataProvider = new PlayerDataProvider();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Called OnPlayerPropertiesUpdate");

        var playerMothBatActionType = playerDataProvider.TryProvideMothBatActionType(targetPlayer, changedProps);

        if (playerMothBatActionType != null)
        {
            Debug.Log($"OnPlayerPropertiesUpdate: " +
                $"ActorNumber:{playerMothBatActionType.ActorNumber} " +
                $"AttackType:{playerMothBatActionType.AttackType} " +
                $"MothBatType:{playerMothBatActionType.MothBatType}");

            if(playerMothBatActionType.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                TestPostprocessing.SetActive(true);
            }
        }
    }

    public void SetLocalPlayerMothBatActionType(MothBatActionType mothBatActionType)
    {
        string serializedValue = Serializer.ObjectToString(mothBatActionType);

        var props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_MOTH_BAT_ACTION_TYPE, serializedValue } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
