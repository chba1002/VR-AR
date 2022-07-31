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
    private bool localPlayerIsBat;
    [SerializeField]
    private PostProcessExecutor postProcessExecutor;

    void Start()
    {
        playerDataProvider = new PlayerDataProvider();
        var batPlayerData = PhotonNetwork.PlayerList
                    .ToList()
                    .Select(player => playerDataProvider.Provide(player))
                    .FirstOrDefault(playerData => playerData?.PlayerMothBatState?.MothBatType == MothBatType.Bat.GetHashCode());

        localPlayerIsBat = PhotonNetwork.LocalPlayer.ActorNumber == batPlayerData?.ActorNumber;

        if (postProcessExecutor == null)
        {
            Debug.LogWarning("PostProcessExecutor isnt set in MothBatNetworkSynchronizer");
            return;
        }

        if (localPlayerIsBat)
        {
            postProcessExecutor.SetPostProcessing(MothBatPostProcessingType.BatDefault);
        }
        else
        {
            postProcessExecutor.SetPostProcessing(MothBatPostProcessingType.MothDefault);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Called OnPlayerPropertiesUpdate");

        var playerMothBatActionType = playerDataProvider.TryProvideMothBatActionType(targetPlayer, changedProps);
        var playerMothBatIsAlive = playerDataProvider.TryProvidePlayerIsAlive(targetPlayer, changedProps);

        if(playerMothBatIsAlive != null && playerMothBatIsAlive.Value == false)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
            {
                postProcessExecutor.SetPostProcessing(MothBatPostProcessingType.MothDead);
            }
        }

        if (playerMothBatActionType != null)
        {

            Debug.Log($"OnPlayerPropertiesUpdate: " +
                $"ActorNumber:{playerMothBatActionType.ActorNumber} " +
                $"AttackType:{playerMothBatActionType.AttackType} " +
                $"MothBatType:{playerMothBatActionType.MothBatType}");

            if (localPlayerIsBat)
            {
                if (playerMothBatActionType.AttackType == AttackType.DisturbBatFieldOfView)
                {
                    postProcessExecutor.SetPostProcessing(5, MothBatPostProcessingType.BatFieldOfViewRestricted);
                }
            }
            else
            {

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
