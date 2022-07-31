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
        var playerMothBatActionType = playerDataProvider.TryProvideMothBatActionType(targetPlayer, changedProps);
        var playerMothBatIsAlive = playerDataProvider.TryProvidePlayerIsAlive(targetPlayer, changedProps);
        var playerMothBatIsInvulnerable = playerDataProvider.TryProvidePlayerIsInvulnerable(targetPlayer, changedProps);

        if (playerMothBatIsAlive != null && playerMothBatIsAlive.Value == false)
        {
            Debug.Log($"Moth with actor number {targetPlayer.ActorNumber} was killed");

            if(PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
            {
                postProcessExecutor.SetPostProcessing(MothBatPostProcessingType.MothDead);
            }

            bool anyMothIsAlive = PhotonNetwork.PlayerList
                .ToList()
                .Select(player => playerDataProvider.Provide(player))
                .Where(playerData => playerData.PlayerMothBatState.MothBatType != MothBatType.Bat.GetHashCode())
                .Any(playerData => playerData.IsAlive.HasValue ? playerData.IsAlive.Value : false);

            if (!anyMothIsAlive)
            {
                Debug.Log("ACHTUNG!!!!! Alle Motten sind tot. Die Fledermaus hat gewonnen!");
                // ToDo: Add here End Game and show bat win.
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
