using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
using Moth.Scripts.Lobby.Managers;
using static OVRInput;

public class MothBatNetworkPlayer : MonoBehaviour
{
    [SerializeField]
    private MothBatType mothBatType;

    /// <summary>
    /// Zeitraum nach einer von Motte / Fledermaus ausgeführten Aktion,
    /// inder diese keine weitere neue Attacke durchführen kann.
    /// </summary>
    [SerializeField]
    private float breakDurationBetweenActionsInSeconds = 5f;

    private float secondsUntillNextActionIsExecutable = 0f;

    public float NextAttackIsReadyInPercent =>
        (breakDurationBetweenActionsInSeconds - secondsUntillNextActionIsExecutable) / breakDurationBetweenActionsInSeconds;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    private MothGameManager mothGameManager;
    private MothBatDistanceIndicatorScript mothBatDistanceIndicatorScript;
    private MothBatAttackIsReadyScript mothBatAttackIsReadyScript;
    // Start is called before the first frame update
    void Start()
    {
        mothGameManager = GameObject
            .FindGameObjectWithTag(TagProvider.MothGameManager)?
            .GetComponent<MothGameManager>();

        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

        if (photonView.IsMine)
        {
            mothBatDistanceIndicatorScript = GameObject.FindObjectOfType<MothBatDistanceIndicatorScript>();
            mothBatAttackIsReadyScript = GameObject.FindObjectOfType<MothBatAttackIsReadyScript>();
            mothGameManager.SetLocalMothBatPlayer(this);
        }

        Debug.Log("mothBatType: " + mothBatType.ToString() + " " + mothBatType.GetHashCode());


        switch (mothBatType)
        {
            case MothBatType.MothBlue:
                mothBatAttackIsReadyScript.gameObject.SetActive(true);
                mothBatDistanceIndicatorScript.gameObject.SetActive(false);
                break;
            case MothBatType.MothOrange:
                mothBatAttackIsReadyScript.gameObject.SetActive(true);
                mothBatDistanceIndicatorScript.gameObject.SetActive(false);
                break;
            case MothBatType.MothPurple:
                mothBatAttackIsReadyScript.gameObject.SetActive(false);
                mothBatDistanceIndicatorScript.gameObject.SetActive(true);
                break;
            case MothBatType.MothGreen:
                mothBatAttackIsReadyScript.gameObject.SetActive(true);
                mothBatDistanceIndicatorScript.gameObject.SetActive(false);
                break;
            case MothBatType.Bat:
                //mothBatAttackIsReadyScript.gameObject.SetActive(true);
               // mothBatDistanceIndicatorScript.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            //head.gameObject.SetActive(false);
            //rightHand.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);

            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            if (secondsUntillNextActionIsExecutable <= 0)
            {
                ExecuteMothBatAction();
            }
            else
            {
                secondsUntillNextActionIsExecutable -= Time.deltaTime;
            }

            if(mothBatType != MothBatType.Bat)
            {
                mothBatDistanceIndicatorScript.SetMoth(gameObject);
            }
        }
    }

    private void ExecuteMothBatAction()
    {
        var actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        var primaryOvrTrigegrPressed = OVRInput.GetDown(OVRInput.GetButtonDown.PrimaryHandTrigger);

        if (Input.GetKeyDown("e") || primaryOvrTrigegrPressed)
        {
            Debug.Log("ExecuteMothBatAction !");
            mothGameManager.MothBarExecuteInteraction(actorNumber, mothBatType);
            secondsUntillNextActionIsExecutable = breakDurationBetweenActionsInSeconds;
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
