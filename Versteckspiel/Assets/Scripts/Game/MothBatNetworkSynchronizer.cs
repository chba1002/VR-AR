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

    public int DurationOfInvulneraibilityInSecondsInSeconds = 5;
    public int DurationOfBatFieldOfViewRestrictedInSeconds = 5;

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
        var playerMothBatIsInvulnerable = playerDataProvider.TryProvidePlayerIsInvulnerable(targetPlayer, changedProps);

        if (playerMothBatIsAlive != null && playerMothBatIsAlive.Value == false)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
            {
                postProcessExecutor.SetPostProcessing(MothBatPostProcessingType.MothDead);
            }
        }


        Debug.Log("playerMothBatIsInvulnerable.Value: " + playerMothBatIsInvulnerable);
        /*
        if (playerMothBatIsInvulnerable != null && playerMothBatIsInvulnerable.Value == true)
        {
            int durationOfInvulneraibilityInSeconds = 5;
            Debug.Log($"Player {targetPlayer.NickName} - {targetPlayer.ActorNumber} will be invulnerable (after implementation) for {durationOfInvulneraibilityInSeconds}");
            // ToDo: Toggle Player is invulverable

            var mothGameobjects = GameObject.FindGameObjectsWithTag("Moth");
            mothGameobjects
                .Select(mothGameobject => mothGameobject.GetComponent<MothBatNetworkPlayer>())
                .FirstOrDefault(mothBatNetworkPlayer => mothBatNetworkPlayer.PlayerData.ActorNumber == targetPlayer.ActorNumber)
                .SetInvulnerable(durationOfInvulneraibilityInSeconds);

        }
        */
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
                    postProcessExecutor.SetPostProcessing(
                        DurationOfBatFieldOfViewRestrictedInSeconds, 
                        MothBatPostProcessingType.BatFieldOfViewRestricted);
                }
            }
            else
            {

                if (playerMothBatActionType.AttackType == AttackType.MakeMothInvulnerable)
                {
                    int durationOfInvulneraibilityInSeconds = DurationOfInvulneraibilityInSecondsInSeconds;
                    Debug.Log($"Player {targetPlayer.NickName} - {targetPlayer.ActorNumber} will be invulnerable (after implementation) for {durationOfInvulneraibilityInSeconds}");
                    // ToDo: Toggle Player is invulverable

                    var mothGameobjects = GameObject.FindGameObjectsWithTag("Moth");
                    mothGameobjects
                        .Select(mothGameobject => mothGameobject.GetComponent<MothBatNetworkPlayer>())
                        .ToList()
                        .Where(mothBatNetworkPlayer => mothBatNetworkPlayer != null)
                        .FirstOrDefault(mothBatNetworkPlayer => mothBatNetworkPlayer.PlayerData.ActorNumber == targetPlayer.ActorNumber)
                        .SetInvulnerable(durationOfInvulneraibilityInSeconds);
                }
            }
        }

    }

    public void SetLocalPlayerMothBatActionType(MothBatActionType mothBatActionType)
    {
        string serializedValue = Serializer.ObjectToString(mothBatActionType);

        var props = new Hashtable() { { MothGame.PLAYER_MOTH_BAT_ACTION_TYPE, serializedValue } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void SetLocalPlayerInvulnerable(bool isInvulnerable)
    {
        Debug.Log("SetLocalPlayerInvulnerable: " + isInvulnerable);
        var props = new Hashtable() { { MothGame.PLAYER_MOTH_IS_INVULNERABLE, isInvulnerable } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

}
