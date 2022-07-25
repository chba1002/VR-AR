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

    internal void SetSelected(PlayerData playerData, int? optionalPlayerId = null)
    {
        var active = playerData.PlayerIsReady ?? false;

        this.optionalPlayerId = optionalPlayerId;
        Debug.Log($"SetSelected: MothBatId: {MothBatId}, active {active},  optionalPlayerId: {optionalPlayerId}");

        if(playerData.PlayerName == null)
        {
            Debug.LogWarning("playerData.PlayerName is null, but shouln't be.");
            return;
        }

        PlayerName.text = active || optionalPlayerId  == null || playerData.PlayerName == null ? "-" : playerData.PlayerName;
    }

    internal void SetReady(bool ready)
    {
        IsReadyPanel.SetActive(ready);
    }
}
