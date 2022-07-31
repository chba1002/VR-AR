using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Assets.Scripts.Shared.Managers;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;
using System.Linq;

public class MothBatNetworkSynchronizer : MonoBehaviourPunCallbacks
{
    private PlayerDataProvider playerDataProvider;

    [SerializeField]
    private PostProcessExecutor postProcessExecutor;

    void Start()
    {
        playerDataProvider = new PlayerDataProvider();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Called OnPlayerPropertiesUpdate");

        var playerMothBatActionType = playerDataProvider.TryProvideMothBatActionType(targetPlayer, changedProps);

        if (playerMothBatActionType == null)
        {
            return;
        }


        var batPlayerData = PhotonNetwork.PlayerList
                    .ToList()
                    .Select(player => playerDataProvider.Provide(player))
                    .FirstOrDefault(playerData => playerData?.PlayerMothBatState?.MothBatType == MothBatType.Bat.GetHashCode());

        //if(batPlayerData == null)
        //{
        //    Debug.LogWarning("No bat found - couldn't executee OnPlayerPropertiesUpdate");
        //}


        Debug.Log($"OnPlayerPropertiesUpdate: " +
            $"ActorNumber:{playerMothBatActionType.ActorNumber} " +
            $"AttackType:{playerMothBatActionType.AttackType} " +
            $"MothBatType:{playerMothBatActionType.MothBatType}");

        var localPlayerIsBat = PhotonNetwork.LocalPlayer.ActorNumber == batPlayerData?.ActorNumber;

        if (localPlayerIsBat)
        {
            if (playerMothBatActionType.AttackType == AttackType.DisturbBatFieldOfView)
            {
                postProcessExecutor.SetPostProcessing(5, MothBatPostProcessingType.Blur);
            }
        }
        else
        {

        }
    }

    public void SetLocalPlayerMothBatActionType(MothBatActionType mothBatActionType)
    {
        string serializedValue = Serializer.ObjectToString(mothBatActionType);

        var props = new ExitGames.Client.Photon.Hashtable() { { MothGame.PLAYER_MOTH_BAT_ACTION_TYPE, serializedValue } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
