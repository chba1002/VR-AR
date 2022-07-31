using UnityEngine;

public class SelectionMenueController : MonoBehaviour
{
    [SerializeField]
    private GameObject SelectedPlayerPanel;

    [SerializeField]
    private GameObject MothSelectionFieldPrefab;

    [Header("Moth Blue")]
    [SerializeField] public Sprite mothImage1;
    [SerializeField] private string mothName1;

    [Header("Moth Orange")]
    [SerializeField] public Sprite mothImage2;
    [SerializeField] private string mothName2;

    [Header("Moth Purple")]
    [SerializeField] public Sprite mothImage3;
    [SerializeField] private string mothName3;

    [Header("Moth Green")]
    [SerializeField] public Sprite mothImage4;
    [SerializeField] private string mothName4;

    public void SelectMoth(int mothTypeInt)
    {
        var mothType = (MothBatType)mothTypeInt;

        foreach (Transform child in SelectedPlayerPanel.transform)
        {
            Destroy(child.gameObject);
        }

        var SelectedMothSelectionField = Instantiate(
            MothSelectionFieldPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            SelectedPlayerPanel.transform);


        MothSelectionFieldController selectedFiled = SelectedMothSelectionField.GetComponent<MothSelectionFieldController>();
        switch (mothType)
        {
            case MothBatType.MothGreen: selectedFiled.Initialize(mothImage1, mothName1);
                break;
            case MothBatType.MothOrange: selectedFiled.Initialize(mothImage2, mothName2);
                break;
            case MothBatType.MothBlue: selectedFiled.Initialize(mothImage3, mothName3);
                break;
            case MothBatType.MothPurple: selectedFiled.Initialize(mothImage4, mothName4);
                break;
            default:
                break;
        }
    }
}
