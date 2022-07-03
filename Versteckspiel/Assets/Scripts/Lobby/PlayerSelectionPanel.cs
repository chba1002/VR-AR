using UnityEngine;
using TMPro;

public class PlayerSelectionPanel : MonoBehaviour
{
    [SerializeField]
    private int MothBatId;

    public TMP_Text PlayerName;


    public TMP_Text ButtonText;

    /// <summary>
    /// Gets a value that is equal to the 'MothBatId'.
    /// </summary>
    public int MothBatIdentifier => MothBatId;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void SetSelected(bool active, int? optionalPlayerId = null)
    {
        PlayerName.text = !active || optionalPlayerId  == null ? "-" : "Player " +optionalPlayerId.ToString();
    }
}
