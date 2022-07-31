using Moth.Scripts.Lobby.Types;
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

    [Header("Network")]
    [SerializeField] private MothBatNetworkSynchronizer mothBatNetworkSynchronizer;

    [Header("Hud Menue")]
    [SerializeField] private MothBatAttackIsReadyScript MothBatAttackIsReadyScript;


    private Dictionary<int, Vector3> positions;
    private MothBatNetworkPlayer mothBatNetworkPlayer;

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

    private void Update()
    {
        if(mothBatNetworkPlayer != null){
            bool mothBatAttackIsReadyImageIsInBaseState = 
                mothBatNetworkPlayer.NextAttackIsReadyInPercent == 1.0f 
                && MothBatAttackIsReadyScript.IsReady;

            if (!mothBatAttackIsReadyImageIsInBaseState)
            {
                MothBatAttackIsReadyScript.SetIsReadyInPercent(mothBatNetworkPlayer.NextAttackIsReadyInPercent);
            }
        }
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

    internal void SetLocalMothBatPlayer(MothBatNetworkPlayer mothBatNetworkPlayer)
    {
        this.mothBatNetworkPlayer = mothBatNetworkPlayer;
    }

    internal void MothBarExecuteInteraction(int actorNumber, MothBatType mothBatType)
    {
        Debug.Log($"MothBarExecuteInteraction: Player {actorNumber}, mothBatType {mothBatType}");
        var mothBatActionTyoe = new MothBatActionType { ActorNumber = actorNumber, MothBatType = mothBatType };

        switch (mothBatType)
        {
            case MothBatType.MothBlue:
                mothBatActionTyoe.AttackType = AttackType.MakeMothInvulnerable;
                Debug.Log("ist für x Sekunden unverwundbar");
                break;
            case MothBatType.MothOrange:
                mothBatActionTyoe.AttackType = AttackType.DisturbBatFieldOfView;
                Debug.Log("alle x Sekunden Sichtfeld von Fledermaus mit PostProVol stören");
                break;
            case MothBatType.MothPurple:
                mothBatActionTyoe.AttackType = AttackType.ShowMothBatDistance;
                Debug.Log("Bild von Glühwürmchen leuchtet mit Farbverlauf (weiß zu rot?), wenn Fledermaus näher kommt");
                break;
            case MothBatType.MothGreen:
                mothBatActionTyoe.AttackType = AttackType.ShortenRemainingTime;
                Debug.Log(" kann Spielzeit heruntersetzen (je später im Spiel, desto mehr)," +
                    "am Ende des Spiels (also wenn Zeit um ist oder alle Motten tot) " +
                    "nach ca. 20s werden alle Spieler wieder in Lobby geschickt" +
                    "(siehe Scoreboard, da kannst du gerne auch drüberschaun), ");
                break;
            case MothBatType.Bat:
                break;
            default:
                break;
        }

        mothBatNetworkSynchronizer.SetLocalPlayerMothBatActionType(mothBatActionTyoe);

        //  private void OnDestroy()
        //   {
        //       OnItemSelected -= HandleItemSelected;
        //    }
    }
}
