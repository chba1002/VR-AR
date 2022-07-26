using UnityEngine;
using TMPro;
using Assets.Scripts.Shared.Managers;

public class PlayerSelectionPanel : MonoBehaviour
{
    private int? optionalPlayerId;

    [SerializeField]
    private int MothBatId;

    public TMP_Text PlayerName;

    public TMP_Text ButtonText;

    [SerializeField]
    private GameObject IsReadyPanel;

    /// <summary>
    /// Gets a value that is equal to the 'MothBatId'.
    /// </summary>
    public int MothBatIdentifier => MothBatId;

    public int? OptionalPlayerId => optionalPlayerId;

    // Start is called before the first frame update
    void Awake()
    {
        IsReadyPanel.SetActive(false);
    }

    internal void SetSelected(PlayerData playerData, Photon.Realtime.Player optionalPlayer= null)
    {
        var active = playerData.PlayerIsReady ?? false;

        Debug.Log("NICK NAME:" + optionalPlayer?.NickName);

        this.optionalPlayerId = optionalPlayerId;
        Debug.Log($"SetSelected: MothBatId: {MothBatId}, active {active},  optionalPlayerId: {optionalPlayerId}");

        /*
        if(playerData.PlayerName == null)
        {
            Debug.LogWarning("playerData.PlayerName is null, but shouln't be."); -> Wrong: playerData.PlayerName  can be null, if it is change data and player name didn't change (... player name never change..)
            return;
        }
        */

        Debug.Log($"playerData.PlayerMothBatState.LastMothBatType: {playerData.PlayerMothBatState.LastMothBatType}, playerData.PlayerMothBatState.MothBatType {playerData.PlayerMothBatState.MothBatType}");
        PlayerName.text = active 
            || playerData.PlayerMothBatState.LastMothBatType != 0 && playerData.PlayerMothBatState.MothBatType == 0
            || (optionalPlayer?.ActorNumber == null || playerData.PlayerName == null) && optionalPlayer?.NickName == null 
                ? "-" 
                : optionalPlayer.NickName;
    }

    internal void SetReady(bool ready)
    {
        IsReadyPanel.SetActive(ready);
    }
}
