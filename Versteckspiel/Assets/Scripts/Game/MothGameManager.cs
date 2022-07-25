using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothGameManager : MonoBehaviour
{
    [Header("Moth and Bat StartPositions")]
    [SerializeField] private GameObject StartPositions_1;
    [SerializeField] private GameObject StartPositions_2;
    [SerializeField] private GameObject StartPositions_3;
    [SerializeField] private GameObject StartPositions_4;
    [SerializeField] private GameObject StartPositions_5;

    private Dictionary<int, Vector3> positions;

    void Start()
    {
        positions = new Dictionary<int, Vector3>();

        void AddAndDisable(int id, GameObject go){
            positions.Add(id, go.transform.position);
            go.SetActive(false);
        }

        AddAndDisable(1, StartPositions_1);
        AddAndDisable(2, StartPositions_2);
        AddAndDisable(3, StartPositions_3);
        AddAndDisable(4, StartPositions_4);
        AddAndDisable(5, StartPositions_5);
    }


    public Vector3 GetPositon(int mothBatId)
    {
        switch (mothBatId)
        {
            case 1: return StartPositions_1.transform.position;
            case 2: return StartPositions_2.transform.position;
            case 3: return StartPositions_3.transform.position;
            case 4: return StartPositions_4.transform.position;
            case 100: return StartPositions_5.transform.position;
        }

        Debug.LogError($"Unknown MothBatType {mothBatId}. Couldnt find position.");
        return Vector3.zero;
    }
}
