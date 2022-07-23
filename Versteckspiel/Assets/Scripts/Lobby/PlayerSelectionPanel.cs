using UnityEngine;
using TMPro;

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

    internal void SetSelected(bool active, int? optionalPlayerId = null)
    {
        this.optionalPlayerId = optionalPlayerId;
        Debug.Log($"SetSelected: MothBatId: {MothBatId}, active {active},  optionalPlayerId: {optionalPlayerId}");

        PlayerName.text = !active || optionalPlayerId  == null ? "-" : "Player " +optionalPlayerId.ToString();
    }

    internal void SetReady(bool ready)
    {
        IsReadyPanel.SetActive(ready);
    }
}
