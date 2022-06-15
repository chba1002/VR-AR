using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectionPanel : MonoBehaviour
{
    [SerializeField]
    private int MothBatId;

    public Button SelectionButton;


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

    internal void SetSelected(bool active)
    {
        ButtonText.text = active ? "Ausgewählt" : "Nicht ausgewählt";
    }
}
